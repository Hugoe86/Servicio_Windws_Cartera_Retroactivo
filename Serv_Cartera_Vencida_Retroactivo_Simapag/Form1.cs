using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using SIAC.Constantes;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Globalization;

namespace Serv_Cartera_Vencida_Retroactivo_Simapag
{
    public partial class Form1 : Form
    {
        public DateTime Dtime_Hora_Inicio;
        public TimeSpan TiemS_Tiempo_Transcurrido;

        /////*******************************************************************************************************
        ///// <summary>
        /////   carga los controles en el formulario
        ///// </summary>
        ///// <returns></returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>23-Agosto-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        public Form1()
        {
            InitializeComponent();
        }

        /////*******************************************************************************************************
        ///// <summary>
        /////   genera la informacion de la cartera vencida
        ///// </summary>
        ///// <returns>un datatable con los campos para mostrar accesos e ingresos por año y mes</returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>23-Agosto-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable Dt_Consulta = new DataTable();
            DataTable Dt_Historico = new DataTable();
            DateTime Dtime_Mes_actual = DateTime.Now;
            DateTime Dtime_Fecha_Bd = DateTime.Now;
            DateTime Dtime_Fecha_Dia_Anterior = DateTime.Now;
            Int32 Int_Dias = 0;

            //StreamWriter SW = new StreamWriter("C:\\Servicios_siac\\Historial.txt", true);

            try
            {

                ProBar_Estado.Visible = true;
                ProBar_Estado.Minimum = 1;
                ProBar_Estado.Value = 1;
                ProBar_Estado.Step = 1;


                Dtime_Hora_Inicio = DateTime.Now;
                Txt_Hora_Inicio.Text = Dtime_Hora_Inicio.ToShortTimeString();

               
                //  se consulta el historico de fechas pendientes por ejecutar
                Dt_Historico = Consulta_Existencia_Mes();
                //SW.WriteLine("************************************************************");
                //SW.WriteLine("tabla de fechas ok " + DateTime.Now.ToString());

                if (Dt_Historico != null)
                {
                    foreach (DataRow Registro_Fechas in Dt_Historico.Rows)
                    {
                        Dtime_Fecha_Bd = Convert.ToDateTime(Registro_Fechas["Fecha"].ToString());
                        //Dtime_Fecha_Dia_Anterior = Dtime_Fecha_Dia_Anterior.AddDays(-1);

                        Int_Dias = DateTime.Compare(Dtime_Fecha_Bd, Dtime_Fecha_Dia_Anterior);

                        //  validamos que sea el dia registrado en la base de datos
                        if (Dtime_Mes_actual.Year == Dtime_Fecha_Bd.Year
                            && Dtime_Mes_actual.Month == Dtime_Fecha_Bd.Month
                            && Dtime_Mes_actual.Day == Dtime_Fecha_Bd.Day)
                        {
                            if (Dtime_Mes_actual.Hour >= 9)
                            {

                                //SW.WriteLine("Validacion de hora correcta " + DateTime.Now.ToString());
                                Insertar_Fecha_Inicio_Servicio();

                                Dt_Consulta = Consulta_Cartera_Vencida();
                                //SW.WriteLine("Consulta de la cartera vencida exitosa  " + DateTime.Now.ToString());

                                //Grid_Resultado.DataSource = Dt_Consulta;

                                ProBar_Estado.Maximum = Dt_Consulta.Rows.Count + 2;

                                Stopwatch watch = new Stopwatch();
                                watch.Start();


                                //  se insertan los registros
                                Insertar(Dt_Consulta, ProBar_Estado, Dtime_Fecha_Bd);
                                //SW.WriteLine("insercion exitosa" + DateTime.Now.ToString());    
                                watch.Stop();
                            }
                        }
                        else if (Int_Dias < 0)
                            //  se valida si la fecha es anterior para realizar el proceso se insercion
                        {
                            if (Dtime_Mes_actual.Hour >= 0 
                                && (Dtime_Mes_actual.Hour <= 20 && Dtime_Mes_actual.Minute <= 15))
                            {
                                Insertar_Fecha_Inicio_Servicio();

                                Dt_Consulta = Consulta_Cartera_Vencida();

                                ProBar_Estado.Maximum = Dt_Consulta.Rows.Count + 2;

                                Stopwatch watch = new Stopwatch();
                                watch.Start();


                                //  se insertan los registros
                                Insertar(Dt_Consulta, ProBar_Estado, Dtime_Fecha_Bd);
                                //SW.WriteLine("insercion exitosa" + DateTime.Now.ToString());    
                                watch.Stop();

                            }
                        }
                    }


                }


                Txt_Hora_Termino.Text = DateTime.Now.ToShortTimeString();


                //SW.WriteLine("************************************************************");
                //SW.Close();
            }
            catch (Exception Ex)
            {
                throw new Exception("Error: " + Ex.Message);
            }
        }

        /////*******************************************************************************************************
        ///// <summary>
        ///// Crear el registro de la cartera vencida
        ///// </summary>
        ///// <returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>13-Enero-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private void Insertar(DataTable Dt_Consulta, ProgressBar ProBar_Estado, DateTime Dtime_Fecha_A_Insertar)
        {
            StringBuilder Mi_Sql = new StringBuilder();
            SqlTransaction Obj_Transaccion = null;
            Int64 Cont_Registro = 0;

            try
            {
                DateTime Dtime_Nueva_Fecha = DateTime.Now;
                Dtime_Nueva_Fecha = Dtime_Fecha_A_Insertar.AddDays(1); //   se agrega un dia a la fecha de la base de datos


                using (SqlConnection Obj_Conexion = new SqlConnection(Cls_Constantes.Str_Conexion))
                {
                    Cont_Registro++;
                    Txt_Tiempo.Text = Cont_Registro.ToString();

                    Obj_Conexion.Open();

                    using (SqlCommand Obj_Comando = Obj_Conexion.CreateCommand())
                    {
                        Obj_Transaccion = Obj_Conexion.BeginTransaction();
                        Obj_Comando.Transaction = Obj_Transaccion;
                        Obj_Comando.Connection = Obj_Conexion;

                        foreach (DataRow Registro in Dt_Consulta.Rows)
                        {
                            Mi_Sql.Clear();
                            Mi_Sql.Append("INSERT INTO Ope_Cor_Cc_Cartera_Vencidad_Retroactivo (" +
                                                " Anio" +               //  1
                                                ",Mes" +                //  2
                                                ",Mes_Adeudo" +         //  3
                                                ",Rpu" +                //  4
                                                ",No_cuenta" +          //  5
                                                ",Tarifa" +             //  6
                                                ",Usuario" +            //  7
                                                ",Calle" +              //  8
                                                ",No_Exterior" +        //  9
                                                ",No_Interior" +        //  10
                                                ",Colonia" +            //  11
                                                ",Agua" +               //  12
                                                ",Drenaje" +            //  13
                                                ",Saneamiento" +        //  14
                                                ",Iva" +                //  15
                                                ",Rezago" +             //  16
                                                ",Recargo" +            //  17
                                                ",Otros_Cargos" +       //  18
                                                ",Total_Adeudo" +       //  19
                                                ",Fecha_Creo" +         //  20
                                                ", Recargo_Agua " +     //  21
                                                ", Recargo_Drenaje " +  //  22
                                                ", Recargo_Saneamiento " +  //  23
                                                ", dia" +               //  24
                                                ", Estatus_Predios" +   //  25
                                                ", Estatus_Cortado" +   //  26
                                                ", Estatus_Cobranza" +  //  27
                                                ", Estatus_Requerido" + //  28
                                                ", Convenio" +          //  29
                                                    ")");

                            Mi_Sql.Append(" Values ");
                            Mi_Sql.Append("(");

                            Mi_Sql.Append(Convert.ToDouble(Registro["Año_Consulta"].ToString()).ToString(new CultureInfo("es-MX")));                     //  1
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Mes_Consulta"].ToString()).ToString(new CultureInfo("es-MX")) + "'");       //  2
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Meses_Adeudo"].ToString()).ToString(new CultureInfo("es-MX")) + "'");         //  3
                            Mi_Sql.Append(", '" + Registro["RPU"].ToString() + "'");                //  4
                            Mi_Sql.Append(", '" + Registro["no_cuenta"].ToString() + "'");          //  5
                            Mi_Sql.Append(", '" + Registro["tarifa"].ToString() + "'");             //  6
                            Mi_Sql.Append(", '" + Registro["Usuario"].ToString() + "'");            //  7
                            Mi_Sql.Append(", '" + Registro["Calle"].ToString() + "'");              //  8
                            Mi_Sql.Append(", '" + Registro["Numero_Exterior"].ToString() + "'");    //  9
                            Mi_Sql.Append(", '" + Registro["Numero_Interior"].ToString() + "'");    //  10
                            Mi_Sql.Append(", '" + Registro["Colonia"].ToString() + "'");            //  11
                            Mi_Sql.Append(", '" +Convert.ToDouble( Registro["Agua"].ToString()).ToString(new CultureInfo("es-MX")) + "'");               //  12
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Alcantarillado"].ToString()).ToString(new CultureInfo("es-MX")) + "'");     //  13
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Saneamiento"].ToString()).ToString(new CultureInfo("es-MX")) + "'");        //  14
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Iva"].ToString()).ToString(new CultureInfo("es-MX")) + "'");                //  15
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Rezago"].ToString()).ToString(new CultureInfo("es-MX")) + "'");             //  16
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Recargo"].ToString()).ToString(new CultureInfo("es-MX")) + "'");            //  17
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Otros_Cargos"].ToString()).ToString(new CultureInfo("es-MX")) + "'");       //  18
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Total"].ToString()).ToString(new CultureInfo("es-MX")) + "'");              //  19
                            Mi_Sql.Append(", getdate()");                                           //  20
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Recargo_Agua"].ToString()).ToString(new CultureInfo("es-MX")) + "'");       //  21
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Recargo_Drenaje"].ToString()).ToString(new CultureInfo("es-MX")) + "'");    //  22
                            Mi_Sql.Append(", '" + Convert.ToDouble(Registro["Recargo_Saneamiento"].ToString()).ToString(new CultureInfo("es-MX")) + "'");//  23
                            Mi_Sql.Append(", '" + Dtime_Fecha_A_Insertar.Day.ToString() + "'");               //  24

                            Mi_Sql.Append(", '" + Registro["Estatus_Predio"].ToString() + "'");            //  25
                            Mi_Sql.Append(", '" + Registro["Estatus_Cortado"].ToString() + "'");            //  26
                            Mi_Sql.Append(", '" + Registro["Estatus_Requerido"].ToString() + "'");            //  27
                            Mi_Sql.Append(", '" + Registro["Estatus_Cobranza"].ToString() + "'");            //  28
                            Mi_Sql.Append(", '" + Registro["Convenio"].ToString() + "'");                   //  29

                            Mi_Sql.Append(")");

                            Obj_Comando.CommandText = Mi_Sql.ToString();
                            Obj_Comando.CommandTimeout = 60;
                            Obj_Comando.ExecuteNonQuery();

                            ProBar_Estado.PerformStep();

                        }// fin del foreach




                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  UPDATE A LA FECHA ANTERIOR
                        Mi_Sql.Clear();
                        Mi_Sql.Append(" update Ope_Cor_Cc_Cartera_Vencidad_Fechas set");
                        Mi_Sql.Append(" estatus = 'S'");
                        Mi_Sql.Append(", Fecha_Fin_Servicio = getdate()");
                        Mi_Sql.Append(" where estatus = 'N'");
                        Obj_Comando.CommandText = Mi_Sql.ToString();
                        Obj_Comando.CommandTimeout = 600;
                        Obj_Comando.ExecuteNonQuery();
                        ProBar_Estado.PerformStep();



                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  INSERT CON LA NUEVA FECHA
                        Mi_Sql.Clear();
                        Mi_Sql.Append("INSERT INTO Ope_Cor_Cc_Cartera_Vencidad_Fechas (" +
                                            " Fecha" +              //  1
                                            ", fecha_Creo " +       //  2
                                                ")");

                        Mi_Sql.Append(" Values ");
                        Mi_Sql.Append("(");

                        Mi_Sql.Append("'" + Dtime_Nueva_Fecha.ToString("yyyy-dd-MM") + "'");    //  1
                        Mi_Sql.Append(", getdate()");                                           //  2

                        Mi_Sql.Append(")");

                        Obj_Comando.CommandText = Mi_Sql.ToString();
                        Obj_Comando.CommandTimeout = 60;
                        Obj_Comando.ExecuteNonQuery();
                        ProBar_Estado.PerformStep();

                        //  se ejecuta la transaccion
                        Obj_Transaccion.Commit();

                    }// fin using obj_comand


                    Obj_Conexion.Close();

                }// fin usaing Obj_conexion

            }
            catch (Exception Ex)
            {
                Obj_Transaccion.Rollback();
                throw new Exception("Error: " + Ex.Message);
            }
        }


        /////*******************************************************************************************************
        ///// <summary>
        ///// Crear el registro de la cartera vencida
        ///// </summary>
        ///// <returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>13-Enero-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private void Insertar_Fecha_Inicio_Servicio()
        {
            StringBuilder Mi_Sql = new StringBuilder();
            SqlTransaction Obj_Transaccion = null;
            Int64 Cont_Registro = 0;

            try
            {
               

                using (SqlConnection Obj_Conexion = new SqlConnection(Cls_Constantes.Str_Conexion))
                {
                  
                    Obj_Conexion.Open();

                    using (SqlCommand Obj_Comando = Obj_Conexion.CreateCommand())
                    {
                        Obj_Transaccion = Obj_Conexion.BeginTransaction();
                        Obj_Comando.Transaction = Obj_Transaccion;
                        Obj_Comando.Connection = Obj_Conexion;

                       


                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  ***************************************************************************************
                        //  UPDATE A LA FECHA ANTERIOR
                        Mi_Sql.Clear();
                        Mi_Sql.Append(" update Ope_Cor_Cc_Cartera_Vencidad_Fechas set");
                        Mi_Sql.Append(" Fecha_Inicio_Servicio = getdate()");
                        Mi_Sql.Append(" where estatus = 'N'");
                        Obj_Comando.CommandText = Mi_Sql.ToString();
                        Obj_Comando.CommandTimeout = 60;
                        Obj_Comando.ExecuteNonQuery();



                        //  se ejecuta la transaccion
                        Obj_Transaccion.Commit();

                    }// fin using obj_comand


                    Obj_Conexion.Close();

                }// fin usaing Obj_conexion

            }
            catch (Exception Ex)
            {
                Obj_Transaccion.Rollback();
                throw new Exception("Error: " + Ex.Message);
            }
        }

        /////*******************************************************************************************************
        ///// <summary>
        ///// Crear el registro de la cartera vencida
        ///// </summary>
        ///// <returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>13-Enero-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private void Insertar_Proxima_Fecha(DateTime Dtime_Fecha, String Id)
        {
            StringBuilder Mi_Sql = new StringBuilder();
            SqlTransaction Obj_Transaccion = null;
          

            try
            {

                using (SqlConnection Obj_Conexion = new SqlConnection(Cls_Constantes.Str_Conexion))
                {

                    Obj_Conexion.Open();

                    using (SqlCommand Obj_Comando = Obj_Conexion.CreateCommand())
                    {
                        Obj_Transaccion = Obj_Conexion.BeginTransaction();
                        Obj_Comando.Transaction = Obj_Transaccion;
                        Obj_Comando.Connection = Obj_Conexion;

                        
                        Obj_Transaccion.Commit();

                    }
                }

            }
            catch (Exception Ex)
            {
                Obj_Transaccion.Rollback();
                throw new Exception("Error: " + Ex.Message);
            }
        }


        /////*******************************************************************************************************
        ///// <summary>
        ///// consulta la cartera vencida
        ///// </summary>
        ///// <returns>un datatable con los campos para mostrar accesos e ingresos por año y mes</returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>23-Agosto-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private DataTable Consulta_Cartera_Vencida()
        {
            DataTable Dt_Consulta = new DataTable();
            StringBuilder Mi_Sql = new StringBuilder();
            DataSet ds;
            SqlDataAdapter da;


            try
            {

                using (SqlConnection conexion = new SqlConnection(Cls_Constantes.Str_Conexion))
                {

                    conexion.Open();

                    using (SqlCommand comando = conexion.CreateCommand())
                    {
                        Mi_Sql.Append("");

                        Mi_Sql.Append("SELECT top 100 ");
                        Mi_Sql.Append(" COUNT(DISTINCT (f.Periodo_Facturacion)) AS Meses_Adeudo");
                        Mi_Sql.Append(", year(GETDATE()) as Año_Consulta");
                        Mi_Sql.Append(", MONTH(GETDATE()) as Mes_Consulta");

                        Mi_Sql.Append(",(" +
                                            " SELECT TOP (1) fs.Fecha_Emision" +
                                            " FROM Ope_Cor_Facturacion_Recibos fs" +
                                            " WHERE fs.Predio_ID = p.Predio_ID" +
                                            " ORDER BY fs.No_Factura_Recibo DESC" +
                                        ") AS Ultimo_Periodo_Facturado");

                        Mi_Sql.Append(", p.Predio_ID");
                        Mi_Sql.Append(", p.RPU AS Rpu");
                        Mi_Sql.Append(", p.No_Cuenta AS No_Cuenta");
                        Mi_Sql.Append(", (t.Abreviatura ) AS Tarifa");
                        Mi_Sql.Append(", REPLACE((ISNULL(u.APELLIDO_PATERNO, '') + ' ' + ISNULL(u.APELLIDO_MATERNO, '') + ' '  + ISNULL(u.NOMBRE, '')), '''', ' ') AS Usuario");
                        Mi_Sql.Append(", ca.NOMBRE AS Calle");
                        Mi_Sql.Append(", u.NO_EXTERIOR as Numero_Exterior");
                        Mi_Sql.Append(", u.NO_INTERIOR as Numero_Interior");
                        Mi_Sql.Append(", c.NOMBRE AS Colonia");
                        Mi_Sql.Append(", p.Estatus as Estatus_Predio");
                        Mi_Sql.Append(", p.Cortado as Estatus_Cortado");
                        Mi_Sql.Append(", p.Requerido as Estatus_Requerido");
                        Mi_Sql.Append(", p.Cobranza as Estatus_Cobranza");

                        Mi_Sql.Append(", case " +
                                        " when ( " +
                                            " select count(conv.No_Convenio)  " +
                                                " from Ope_Cor_Convenios conv " +
                                                " where conv.Predio_ID = p.Predio_ID " +
                                                " and conv.Estatus = 'PENDIENTE'" +
                                            ") >= 1 then 1" +
                                            " ELSE 0 " +                                            
                                        " end as Convenio"); 

                        //**************************************************************************************************************
                        //  total
                        //**************************************************************************************************************
                        Mi_Sql.Append(", cast(isnull(( " +
                                            " SELECT sum(frd.Total_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    " 'PENDIENTE'" +
                                                    ", 'PARCIAL'" +
                                                    ")" +
                                                " AND fr.Predio_ID = p.Predio_ID" +
                                        " ), 0) as DOUBLE PRECISION) AS Total");
                        //**************************************************************************************************************
                        //  agua
                        //**************************************************************************************************************
                        Mi_Sql.Append(",( " +
                                        "ISNULL(" +
                                            "(" +
                                                " SELECT SUM(frd.Importe_Saldo)" +
                                                " FROM Ope_Cor_Facturacion_Recibos fr" +
                                                " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                                " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                                " WHERE fr.Estatus_Recibo IN (" +
                                                        "'PENDIENTE'" +
                                                        ",'PARCIAL'" +
                                                        ")" +
                                                    " AND fr.RPU = p.RPU" +
                                                    " AND co.Concepto_ID = (" +
                                                        " SELECT CONCEPTO_AGUA" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                "), 0) + ISNULL((" +
                                                " SELECT SUM(frd.Importe_Saldo)" +
                                                " FROM Ope_Cor_Facturacion_Recibos fr" +
                                                " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                                " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                                " WHERE fr.Estatus_Recibo IN (" +
                                                        "'PENDIENTE'" +
                                                        ",'PARCIAL'" +
                                                        ")" +
                                                    " AND fr.RPU = p.RPU" +
                                                    " AND co.Concepto_ID = (" +
                                                        " SELECT Cat_Cor_Parametros.Concepto_Agua_Comercial" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                "), 0)" +
                                        ") AS [Agua]");

                        //**************************************************************************************************************
                        //  drenaje
                        //**************************************************************************************************************
                        Mi_Sql.Append(",(" +
                                            " ISNULL((" +
                                                    " SELECT SUM(frd.Importe_Saldo)" +
                                                    " FROM Ope_Cor_Facturacion_Recibos fr" +
                                                    " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                                    " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                                    " WHERE fr.Estatus_Recibo IN (" +
                                                            "'PENDIENTE'" +
                                                            ",'PARCIAL'" +
                                                            ")" +
                                                        " AND fr.RPU = p.RPU" +
                                                        " AND co.Concepto_ID = (" +
                                                            " SELECT CONCEPTO_DRENAJE" +
                                                            " FROM Cat_Cor_Parametros" +
                                                            ")" +
                                                    "), 0) " +
                                            ") AS [Alcantarillado]");

                        //**************************************************************************************************************
                        //  saneamiento
                        //**************************************************************************************************************
                        Mi_Sql.Append(",(" +
                                            " ISNULL((" +
                                                    " SELECT SUM(frd.Importe_Saldo)" +
                                                    " FROM Ope_Cor_Facturacion_Recibos fr" +
                                                    " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                                    " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                                    " WHERE fr.Estatus_Recibo IN (" +
                                                            "'PENDIENTE'" +
                                                            ",'PARCIAL'" +
                                                            ")" +
                                                        " AND fr.RPU = p.RPU" +
                                                        " AND co.Concepto_ID = (" +
                                                            " SELECT CONCEPTO_SANAMIENTO" +
                                                            " FROM Cat_Cor_Parametros" +
                                                            ")" +
                                                    "), 0)" +
                                            ") AS [Saneamiento]");
                        //**************************************************************************************************************
                        //  iva
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                        " SELECT SUM(frd.Impuesto_Saldo)" +
                                        " FROM Ope_Cor_Facturacion_Recibos fr" +
                                        " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                        " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                        " WHERE fr.Estatus_Recibo IN (" +
                                                "'PENDIENTE'" +
                                                ",'PARCIAL'" +
                                                ")" +
                                            " AND fr.RPU = p.RPU" +
                                        "), 0) AS [IVA]");

                        //**************************************************************************************************************
                        //  recargo
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                            " SELECT SUM(frd.Importe_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    "'PENDIENTE'" +
                                                    ",'PARCIAL'" +
                                                    ")" +
                                                " AND fr.RPU = p.RPU" +
                                                " AND (" +
                                                    " co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Recargo_Agua_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    " OR co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Recargo_Drenaje_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    " OR co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Recargo_Saneamiento_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    ")" +
                                            "), 0) AS [Recargo]");

                        //**************************************************************************************************************
                        //  recargo agua
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                            " SELECT SUM(frd.Importe_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    "'PENDIENTE'" +
                                                    ",'PARCIAL'" +
                                                    ")" +
                                                " AND fr.RPU = p.RPU" +
                                                " AND co.Concepto_ID IN (" +
                                                    " SELECT Concepto_Recargo_Agua_Id" +
                                                    " FROM Cat_Cor_Parametros" +
                                                    ")" +
                                            "), 0) AS [Recargo_Agua]");

                        //**************************************************************************************************************
                        //  recargo drenaje
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                            " SELECT SUM(frd.Importe_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    "'PENDIENTE'" +
                                                    ",'PARCIAL'" +
                                                    ")" +
                                                " AND fr.RPU = p.RPU" +
                                                " AND co.Concepto_ID IN (" +
                                                    " SELECT Concepto_Recargo_Drenaje_Id" +
                                                    " FROM Cat_Cor_Parametros" +
                                                    ")" +
                                            "), 0) AS [Recargo_Drenaje]");

                        //**************************************************************************************************************
                        //  recargo Saneamiento
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                            " SELECT SUM(frd.Importe_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    "'PENDIENTE'" +
                                                    ",'PARCIAL'" +
                                                    ")" +
                                                " AND fr.RPU = p.RPU" +
                                                " AND co.Concepto_ID IN (" +
                                                    " SELECT Concepto_Recargo_Saneamiento_Id" +
                                                    " FROM Cat_Cor_Parametros" +
                                                    ")" +
                                            "), 0) AS [Recargo_Saneamiento]");

                        //**************************************************************************************************************
                        //  rezago
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                            " SELECT SUM(frd.Importe_Saldo)" +
                                            " FROM Ope_Cor_Facturacion_Recibos fr" +
                                            " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                            " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                            " WHERE fr.Estatus_Recibo IN (" +
                                                    "'PENDIENTE'" +
                                                    ",'PARCIAL'" +
                                                    ")" +
                                                " AND fr.RPU = p.RPU" +
                                                " AND (" +
                                                    "co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Rezago_Agua_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    " OR co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Rezago_Drenaje_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    " OR co.Concepto_ID IN (" +
                                                        " SELECT Concepto_Rezago_Saneamiento_Id" +
                                                        " FROM Cat_Cor_Parametros" +
                                                        ")" +
                                                    " or co.Concepto_ID in (SELECT Concepto_Rezago_Agua_Comercial_Id from Cat_Cor_Parametros)" +
                                                    ")" +
                                            "), 0) AS [Rezago]");

                        //**************************************************************************************************************
                        //  otros cargos
                        //**************************************************************************************************************
                        Mi_Sql.Append(",ISNULL((" +
                                        " SELECT SUM(frd.Importe_Saldo)" +
                                        " FROM Ope_Cor_Facturacion_Recibos fr" +
                                        " JOIN Ope_Cor_Facturacion_Recibos_Detalles frd ON fr.No_Factura_Recibo = frd.No_Factura_Recibo" +
                                        " JOIN Cat_Cor_Conceptos_Cobros co ON frd.Concepto_ID = co.Concepto_ID" +
                                        " WHERE fr.Estatus_Recibo IN (" +
                                                "'PENDIENTE'" +
                                                ",'PARCIAL'" +
                                                ")" +
                                            " AND fr.RPU = p.RPU" +
                                            " AND (" +
                                                " co.Nombre NOT LIKE '%recargo%'" +
                                                " AND co.Nombre NOT LIKE '%rezago%'" +
                                                " AND co.Nombre NOT LIKE '%agua'" +
                                                " AND co.Nombre NOT LIKE '%AGUA COMERCIAL'" +
                                                " AND co.Nombre NOT LIKE '%drenaje'" +
                                                " AND co.Nombre NOT LIKE '%saneamiento'" +
                                                " AND co.Nombre NOT LIKE '%iva'" +
                                                ")" +
                                        "), 0) AS [Otros_Cargos]");

                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //  from
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        Mi_Sql.Append(" FROM Cat_Cor_Predios p " +
                                            " LEFT OUTER JOIN Ope_Cor_Facturacion_Recibos f ON f.Predio_ID = p.Predio_ID" +
                                            " LEFT OUTER JOIN Ope_Cor_Facturacion_Recibos_Detalles fd ON fd.No_Factura_Recibo = f.No_Factura_Recibo" +
                                            " LEFT OUTER JOIN Cat_Cor_Usuarios u ON p.Usuario_ID = u.USUARIO_ID" +
                                            " LEFT OUTER JOIN Cat_Cor_Tarifas t ON t.Tarifa_ID = p.Tarifa_ID" +
                                            " LEFT OUTER JOIN Cat_Cor_Regiones r ON r.Region_ID = p.Region_ID" +
                                            " LEFT OUTER JOIN Cat_Cor_Colonias c ON c.COLONIA_ID = p.Colonia_ID" +
                                            " LEFT OUTER JOIN Cat_Cor_Calles ca ON ca.CALLE_ID = p.Calle_ID");


                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //  where
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //Mi_Sql.Append(" WHERE f.Estatus_Recibo IN (" +
                        //                "'PENDIENTE'" +
                        //                ",'PARCIAL'" +
                        //                ")");

                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //  GROUP BY 
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        Mi_Sql.Append(" GROUP BY ");
                        Mi_Sql.Append("  p.Region_ID");
                        Mi_Sql.Append(", p.Sector_ID");
                        Mi_Sql.Append(", p.Colonia_ID");
                        Mi_Sql.Append(", p.Predio_ID");
                        Mi_Sql.Append(", p.No_Cuenta");
                        Mi_Sql.Append(", p.RPU");
                        Mi_Sql.Append(", r.Region_ID");
                        Mi_Sql.Append(", r.Numero_Region");
                        Mi_Sql.Append(", p.No_Orden_Reparto");
                        Mi_Sql.Append(", c.NOMBRE");
                        Mi_Sql.Append(", ca.NOMBRE");
                        Mi_Sql.Append(", u.NO_EXTERIOR");
                        Mi_Sql.Append(", u.NO_INTERIOR");
                        Mi_Sql.Append(", u.NOMBRE");
                        Mi_Sql.Append(", u.APELLIDO_PATERNO");
                        Mi_Sql.Append(", u.APELLIDO_MATERNO");
                        Mi_Sql.Append(", t.Abreviatura");
                        Mi_Sql.Append(", p.Estatus ");
                        Mi_Sql.Append(", p.Cortado ");
                        Mi_Sql.Append(", p.Requerido");
                        Mi_Sql.Append(", p.Cobranza");

                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        //  ORDER BY 
                        //**************************************************************************************************************
                        //**************************************************************************************************************
                        Mi_Sql.Append(" ORDER BY ");
                        Mi_Sql.Append(" p.no_cuenta ASC");
                       

                        comando.CommandText = Mi_Sql.ToString();
                        comando.CommandTimeout = 200;
                        da = new SqlDataAdapter(comando);
                        ds = new DataSet();
                        da.Fill(ds);

                        Dt_Consulta = ds.Tables[0];

                    }
                }


            }
            catch (Exception Ex)
            {
                throw new Exception("Error: " + Ex.Message);
            }
            return Dt_Consulta;

        }

        /////*******************************************************************************************************
        ///// <summary>
        ///// consulta la cartera vencida
        ///// </summary>
        ///// <returns>un datatable con los campos para mostrar accesos e ingresos por año y mes</returns>
        ///// <creo>Hugo Enrique Ramírez Aguilera</creo>
        ///// <fecha_creo>23-Agosto-2016</fecha_creo>
        ///// <modifico></modifico>
        ///// <fecha_modifico></fecha_modifico>
        ///// <causa_modificacion></causa_modificacion>
        ///*******************************************************************************************************
        private DataTable Consulta_Existencia_Mes()
        {
            DataTable Dt_Consulta = new DataTable();
            StringBuilder Mi_Sql = new StringBuilder();
            DataSet ds;
            SqlDataAdapter da;

            try
            {

                using (SqlConnection conexion = new SqlConnection(Cls_Constantes.Str_Conexion))
                {

                    conexion.Open();

                    using (SqlCommand comando = conexion.CreateCommand())
                    {
                        Mi_Sql.Append("");

                        Mi_Sql.Append("SELECT *");
                        Mi_Sql.Append(" from Ope_Cor_Cc_Cartera_Vencidad_Fechas");
                        Mi_Sql.Append(" where ESTATUS = 'N' ");

                        comando.CommandText = Mi_Sql.ToString();
                        comando.CommandTimeout = 100;
                        da = new SqlDataAdapter(comando);
                        ds = new DataSet();
                        da.Fill(ds);

                        Dt_Consulta = ds.Tables[0];

                    }
                }


            }
            catch (Exception Ex)
            {
                throw new Exception("Error: " + Ex.Message);
            }
            return Dt_Consulta;

        }


    }
}