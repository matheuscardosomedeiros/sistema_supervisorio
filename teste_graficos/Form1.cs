using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace teste_graficos
{
    public partial class Form1 : Form
    {
        private static String strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=supervisor.mdb";
        OleDbConnection conn = new OleDbConnection(strConnection);
        OleDbCommand strSQL;
        OleDbDataReader result;

        int x = 0;
        int y = 0;
        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            Init_DB();
            label13.Text = "";
            label14.Text = "";
            label15.Text = "";
            timer3.Start();
            dataGridView1.DataSource = getDT();
        }

        private void Init_DB()
        {
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar-se ao banco de dados: " + ex.Message);
            }
        }

        private void disconnect()
        {
            conn.Close();
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort("COM3", 9600);

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir a porta serial: " + ex.Message);
            }
        }

        private void insert_data(string sensor_name, string level, string actuator_state)
        {
            String aux = "insert into sensors_tab([Sensor], [Level], [Actuator], [Date_time]) values (@sensor, @level, @actuator, @date_time)";
            strSQL = new OleDbCommand(aux, conn);

            strSQL.Parameters.Add("@sensor", OleDbType.VarChar).Value = sensor_name;
            strSQL.Parameters.Add("@level", OleDbType.VarChar).Value = level;
            strSQL.Parameters.Add("@actuator", OleDbType.VarChar).Value = actuator_state;
            strSQL.Parameters.Add("@date_time", OleDbType.VarChar).Value = DateTime.Now.ToString();
            try
            {
                strSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private DataTable getDT()
        {
            OleDbDataAdapter adapter;
            DataTable dt = new DataTable();

            String aux = "select * from sensors_tab";

            adapter = new OleDbDataAdapter(aux, conn);
            adapter.Fill(dt);
            adapter.Dispose();
            return dt;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            serialPort.Write("a");
            string received_data = serialPort.ReadLine();
            x++;
            label1.Text = $"Time: {x}s";

            chart1.Series["Luminosity"].Points.AddXY(x, received_data);
            listBox1.Items.Add($"({x}, {received_data})");
            label4.Text = $"Level: {received_data}";
            

            int init_threshold = int.Parse(textBox1.Text);
            int final_threshold = int.Parse(textBox2.Text);
            int val = int.Parse(received_data);
            
        
            if (val >= init_threshold && val <= final_threshold)
            {
                serialPort.Write("0");
                pictureBox1.Image = teste_graficos.Properties.Resources.servo_90_deg;
                insert_data("LDR", received_data, "90");
            }
            else
            {
                serialPort.Write("1");
                pictureBox1.Image = teste_graficos.Properties.Resources.servo_0_deg;
                insert_data("LDR", received_data, "0");
            }

            dataGridView1.DataSource = getDT();
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            serialPort.Write("b");
            string received_dist = serialPort.ReadLine();
            y++;
            label7.Text = $"Time: {y}s";

            chart2.Series["Distance"].Points.AddXY(y, received_dist);
            listBox2.Items.Add($"({y}, {received_dist})");
            label8.Text = $"Distance: {received_dist}cm";

            int setpoint = int.Parse(textBox3.Text);

            if (int.Parse(received_dist) <= setpoint)
            {
                serialPort.WriteLine("c");
                insert_data("HCSR04", received_dist, "Tone");
            }
            else
            {
                serialPort.WriteLine("d");
                insert_data("HCSR04", received_dist, "NoTone");
            }

            dataGridView1.DataSource = getDT();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            label13.Text = DateTime.Now.ToString();
            label14.Text = DateTime.Now.ToString();
            label15.Text = DateTime.Now.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer2.Stop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            disconnect();
        }

    }
}
