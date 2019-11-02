using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aprirori
{
    public partial class Form1 : Form
    {
        Apriori newTask;
        string path;
        public Form1()
        {
            InitializeComponent();
            comboBox_Measurement.SelectedIndex = 0;
        }

        private void emptyControls()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox_Measurement.SelectedIndex = 0;
        }

        private void button_FileSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "")
            {
                path = fileDialog.FileName;
                label_FileName.Text = "File: " + fileDialog.SafeFileName;
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool flag = true;
            if(textBox1.Text.Length>0 && textBox2.Text.Length>0)
            { 
                if(float.TryParse(textBox1.Text,out float res1) && float.TryParse(textBox2.Text, out float res2))
                {
                    float minSupport = float.Parse(textBox1.Text);
                    float measur_value = float.Parse(textBox2.Text);
                    if (minSupport < 0 || minSupport > 1)
                    {
                        MessageBox.Show("Minimum support takes value between 0 to 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        emptyControls();
                        flag = false;
                    }
                    if (comboBox_Measurement.SelectedIndex == 0 && (measur_value < 0 || measur_value > 1))
                    {
                        MessageBox.Show("Confidence takes value between 0 to 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        emptyControls();
                        flag = false;
                    }   
                    if (comboBox_Measurement.SelectedIndex == 1 && (measur_value < 0))
                    {
                        MessageBox.Show("Lift must be bigger than 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        emptyControls();
                        flag = false;
                    }
                    if (comboBox_Measurement.SelectedIndex == 2 && (measur_value < -0.25 || measur_value > 0.25))
                    {
                        MessageBox.Show("Leverage takes value between -0.25 to 0.25", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        emptyControls();
                        flag = false;
                    }
                    if(flag)
                    {
                        newTask = new Apriori(minSupport, measur_value, comboBox_Measurement.SelectedItem.ToString());
                        newTask.takeInput(path);
                        newTask.startApriori();
                        richTextBox1.Text = newTask.output;
                    }
                }
                else
                {
                    emptyControls();
                }
            }
            else
            {
                MessageBox.Show("Please fill all inputs","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
