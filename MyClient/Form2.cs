using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;// файлы

namespace MyClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)// Сохранить настройки 
        {
            if(textBox1.Text != "" && textBox1.Text != " " && textBox2.Text != "" && textBox2.Text != " ")// если поле не пустое 
            {
                try// обработчик ошибок 
                {
                    DirectoryInfo data = new DirectoryInfo("Client_info");// создать папку 
                    data.Create();// создать папку 

                    var sw = new StreamWriter(@"Client_info/data_info.txt");// создать поток в папку 
                    sw.WriteLine(textBox1.Text + ":" + textBox2.Text);// запись 
                    sw.Close();// закрыть поток 
                    this.Hide();// скрыть окно 
                    Application.Restart();// перезапуск проги с норм. насройками
                }
                catch(Exception ex)// если из try ничего не сработало 
                {
                    MessageBox.Show("Ошибка: " + ex.Message);// сообщеие 
                }
            }
        }
    }
}
