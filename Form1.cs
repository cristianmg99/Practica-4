using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Practica_4
{
	public partial class Form1 : Form
	{
		SerialPort Lora;
		String mensaje;
		Timer timerControl;


		string[] ValoresSensores;
		string Valores;

		public Form1()
		{
			InitializeComponent();
			ValoresSensores = new string[10];


			Lora= new SerialPort();
			Lora.BaudRate = 9600;
			Lora.PortName = "COM11";
			Lora.DataReceived += Lora_DataReceived;


			//Timer
			timerControl = new Timer();
			timerControl.Interval = 250;
			timerControl.Enabled = true;
			timerControl.Tick += TimerControl_Tick;
			timerControl.Stop();
		}

		private void TimerControl_Tick(object sender, EventArgs e)
		{
			try
			{
				//txt_Cadena.Text = mensaje;
				if (ValoresSensores[0] == "~1")
				{
					lbl_Sensor1.Text = ValoresSensores[1];
					Bar_S1.Value = Convert.ToInt32(ValoresSensores[2]);
					Bar_S1.Text = ValoresSensores[2];


					/*lbl_Sensor2.Text = ValoresSensores[3];
					Bar_S2.Value = Convert.ToInt32(ValoresSensores[4]);
					Bar_S2.Text = ValoresSensores[4];*/

					/*lbl_Sensor3.Text = ValoresSensores[5];
					Bar_S3.Value = Convert.ToInt32(ValoresSensores[6]);
					Bar_S3.Text = ValoresSensores[6];*/

				}

				if (ValoresSensores[0] == "~2")
				{
					lbl_Sensor3.Text = ValoresSensores[1];
					Bar_S3.Value = Convert.ToInt32(ValoresSensores[2]);
					Bar_S3.Text = ValoresSensores[2];
				}

				if (ValoresSensores[0] == "~3")
				{
					lbl_Sensor2.Text = ValoresSensores[1];
					Bar_S2.Value = Convert.ToInt32(ValoresSensores[2]);
					Bar_S2.Text = ValoresSensores[2];
				}

				/*if (ValoresSensores[0] == "~01")
				{
					lbl_Unidad.Text = ValoresSensores[1];
				    lbl_Canal.Text = ValoresSensores[5];
					
					lbl_direccion.Text = ValoresSensores[6];
					
				
				}*/

				if (ValoresSensores[0] == "~03")
				{
					txt_Cadena2.Text = mensaje;
				}
				if (ValoresSensores[0] == "~01")
				{
					txt_Cadena1.Text = mensaje;
				}




			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				timerControl.Stop();
			}
			
			
		}

		private void Lora_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SolicitarValores(2);
		}

		private void btn_Configuraciones_Click(object sender, EventArgs e)
		{
			if (Lora.IsOpen)
			{
				Lora.WriteLine("~$0/25");
				Lora.WriteLine("~%0/25");
				Lora.WriteLine("~&0/25");

			}
		}

		private void btn_Conectar_Click(object sender, EventArgs e)
		{
			if (!Lora.IsOpen && btn_Conectar.Text == "Conectar")
			{
				try
				{
					Lora.Open();
					btn_Conectar.Text = "Desconectar";
					timerControl.Start();


				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + "\nVerificar el COM de su dispositivo", "Fallo del COM");

					timerControl.Stop();

				}

			}
			else if (Lora.IsOpen && btn_Conectar.Text == "Desconectar")
			{
				Lora.Close();
				btn_Conectar.Text = "Conectar";
				timerControl.Stop();
			}
		}

		private void txt_tiempo_KeyPress(object sender, KeyPressEventArgs e)
		{ //Solo permitir numeros en el textbox del tiempo
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void btn_Tiempo_Click(object sender, EventArgs e)
		{
			if (Lora.IsOpen)
			{
				if (chb_unidad1.Checked && chb_unidad3.Checked)
				{
					Lora.WriteLine("~$1/26/" + txt_tiempo.Text);
					Lora.WriteLine("~&3/26/" + txt_tiempo.Text);

					lbl_tiempo.Text = "Tiempo = " + txt_tiempo.Text + "ms";
					txt_tiempo.Text = "";
				}

				if (chb_unidad1.Checked)
				{
					Lora.WriteLine("~$1/26/" + txt_tiempo.Text);
					lbl_tiempo.Text = "Tiempo = " + txt_tiempo.Text + "ms";
					txt_tiempo.Text = "";
				}

				if (chb_unidad3.Checked)
				{
					Lora.WriteLine("~&3/26/" + txt_tiempo.Text);
					lbl_tiempo.Text = "Tiempo = " + txt_tiempo.Text + "ms";
					txt_tiempo.Text = "";
				}
			}

		}

		private void btn_request_Click(object sender, EventArgs e)
		{
			if (Lora.IsOpen)
			{
				if (chb_unidad1.Checked && chb_unidad2.Checked && chb_unidad3.Checked)
				{
					Lora.WriteLine("~$1/20"); 
					Lora.WriteLine("~%2/20");
					Lora.WriteLine("~&3/20");
				}
				else if (chb_unidad1.Checked && chb_unidad2.Checked)
				{
					Lora.WriteLine("~$1/20");
					Lora.WriteLine("~%2/20");
				}
				else if (chb_unidad1.Checked && chb_unidad3.Checked)
				{
					Lora.WriteLine("~$1/20"); 
					Lora.WriteLine("~&3/20");
				}
				else if (chb_unidad2.Checked && chb_unidad3.Checked)
				{
					Lora.WriteLine("~%2/20");
					Lora.WriteLine("~&3/20");
				}
				else if (chb_unidad1.Checked )
				{
					Lora.WriteLine("~$1/20"); 
				}
				else if (chb_unidad2.Checked)
				{
					Lora.WriteLine("~%2/20");
				}
				else if (chb_unidad3.Checked)
				{
					Lora.WriteLine("~&3/20");
				}


			}
		}

		void SolicitarValores(int Length)
		{
			try
			{
				if (Lora.BytesToRead > Length)
				{
					mensaje = Lora.ReadLine();
					ValoresSensores = mensaje.Split('/');
				}
				

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				timerControl.Stop();

			}

		}
	}
}
