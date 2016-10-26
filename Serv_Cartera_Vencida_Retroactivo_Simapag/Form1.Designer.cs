namespace Serv_Cartera_Vencida_Retroactivo_Simapag
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Grid_Resultado = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.ProBar_Estado = new System.Windows.Forms.ProgressBar();
            this.Lbl_Etiqueta_Hora_Inicio = new System.Windows.Forms.Label();
            this.Txt_Hora_Inicio = new System.Windows.Forms.TextBox();
            this.Lbl_Etiqueta_Hora_Termino = new System.Windows.Forms.Label();
            this.Txt_Hora_Termino = new System.Windows.Forms.TextBox();
            this.Txt_Tiempo = new System.Windows.Forms.TextBox();
            this.Lbl_Tiempo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Resultado)).BeginInit();
            this.SuspendLayout();
            // 
            // Grid_Resultado
            // 
            this.Grid_Resultado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid_Resultado.Location = new System.Drawing.Point(12, 137);
            this.Grid_Resultado.Name = "Grid_Resultado";
            this.Grid_Resultado.Size = new System.Drawing.Size(612, 253);
            this.Grid_Resultado.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(184, 42);
            this.button1.TabIndex = 1;
            this.button1.Text = "Crear retroactivo actual";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProBar_Estado
            // 
            this.ProBar_Estado.Location = new System.Drawing.Point(12, 90);
            this.ProBar_Estado.Name = "ProBar_Estado";
            this.ProBar_Estado.Size = new System.Drawing.Size(612, 32);
            this.ProBar_Estado.TabIndex = 2;
            // 
            // Lbl_Etiqueta_Hora_Inicio
            // 
            this.Lbl_Etiqueta_Hora_Inicio.AutoSize = true;
            this.Lbl_Etiqueta_Hora_Inicio.Location = new System.Drawing.Point(235, 13);
            this.Lbl_Etiqueta_Hora_Inicio.Name = "Lbl_Etiqueta_Hora_Inicio";
            this.Lbl_Etiqueta_Hora_Inicio.Size = new System.Drawing.Size(57, 13);
            this.Lbl_Etiqueta_Hora_Inicio.TabIndex = 3;
            this.Lbl_Etiqueta_Hora_Inicio.Text = "Hora inicio";
            // 
            // Txt_Hora_Inicio
            // 
            this.Txt_Hora_Inicio.Location = new System.Drawing.Point(309, 10);
            this.Txt_Hora_Inicio.Name = "Txt_Hora_Inicio";
            this.Txt_Hora_Inicio.Size = new System.Drawing.Size(100, 20);
            this.Txt_Hora_Inicio.TabIndex = 4;
            // 
            // Lbl_Etiqueta_Hora_Termino
            // 
            this.Lbl_Etiqueta_Hora_Termino.AutoSize = true;
            this.Lbl_Etiqueta_Hora_Termino.Location = new System.Drawing.Point(235, 41);
            this.Lbl_Etiqueta_Hora_Termino.Name = "Lbl_Etiqueta_Hora_Termino";
            this.Lbl_Etiqueta_Hora_Termino.Size = new System.Drawing.Size(67, 13);
            this.Lbl_Etiqueta_Hora_Termino.TabIndex = 5;
            this.Lbl_Etiqueta_Hora_Termino.Text = "Hora termino";
            // 
            // Txt_Hora_Termino
            // 
            this.Txt_Hora_Termino.Location = new System.Drawing.Point(309, 38);
            this.Txt_Hora_Termino.Name = "Txt_Hora_Termino";
            this.Txt_Hora_Termino.Size = new System.Drawing.Size(100, 20);
            this.Txt_Hora_Termino.TabIndex = 6;
            // 
            // Txt_Tiempo
            // 
            this.Txt_Tiempo.Location = new System.Drawing.Point(309, 64);
            this.Txt_Tiempo.Name = "Txt_Tiempo";
            this.Txt_Tiempo.Size = new System.Drawing.Size(100, 20);
            this.Txt_Tiempo.TabIndex = 8;
            // 
            // Lbl_Tiempo
            // 
            this.Lbl_Tiempo.AutoSize = true;
            this.Lbl_Tiempo.Location = new System.Drawing.Point(235, 63);
            this.Lbl_Tiempo.Name = "Lbl_Tiempo";
            this.Lbl_Tiempo.Size = new System.Drawing.Size(42, 13);
            this.Lbl_Tiempo.TabIndex = 7;
            this.Lbl_Tiempo.Text = "Tiempo";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 397);
            this.Controls.Add(this.Txt_Tiempo);
            this.Controls.Add(this.Lbl_Tiempo);
            this.Controls.Add(this.Txt_Hora_Termino);
            this.Controls.Add(this.Lbl_Etiqueta_Hora_Termino);
            this.Controls.Add(this.Txt_Hora_Inicio);
            this.Controls.Add(this.Lbl_Etiqueta_Hora_Inicio);
            this.Controls.Add(this.ProBar_Estado);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Grid_Resultado);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Cartera Vencida Retroactivo";
            ((System.ComponentModel.ISupportInitialize)(this.Grid_Resultado)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Grid_Resultado;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar ProBar_Estado;
        private System.Windows.Forms.Label Lbl_Etiqueta_Hora_Inicio;
        private System.Windows.Forms.TextBox Txt_Hora_Inicio;
        private System.Windows.Forms.Label Lbl_Etiqueta_Hora_Termino;
        private System.Windows.Forms.TextBox Txt_Hora_Termino;
        private System.Windows.Forms.TextBox Txt_Tiempo;
        private System.Windows.Forms.Label Lbl_Tiempo;
    }
}

