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
			Lora= new SerialPort();
			Lora.BaudRate = 9600;
			Lora.PortName = "COM3";
			Lora.DataReceived += Lora_DataReceived;


			//Timer
			timerControl = new Timer();
			timerControl.Interval = 250;
			timerControl.Enabled = true;
			timerControl.Tick += TimerControl_Tick;	
		}

		private void TimerControl_Tick(object sender, EventArgs e)
		{
			txt_Cadena.Text = mensaje;
			
		}

		private void Lora_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SolicitarValores(2);
		}

		private void btn_Configuraciones_Click(object sender, EventArgs e)
		{
			if (Lora.IsOpen)
			{
				Lora.WriteLine("~$1/25"); //El $=36  codigo ASCII
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
				Lora.WriteLine("~*1/26/"+txt_tiempo.Text); //El *=42  codigo ASCII
				lbl_tiempo.Text = "Tiempo = " + txt_tiempo.Text +"ms";
				txt_tiempo.Text = "";	
			}

		}

		private void btn_request_Click(object sender, EventArgs e)
		{
			if (Lora.IsOpen)
			{
				Lora.WriteLine("~$1/20"); //El $=42  codigo ASCII
				
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
