using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurityTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FileManager.InitializeFile();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // УЯЗВИМОСТЬ №2: Отсутствует защита от перебора (Брутфорса).
            string role = FileManager.Authenticate(username, password);

            if (role != null)
            {
                MessageBox.Show("Успешный вход!");
                if (role == "Admin")
                {
                    AdminForm admin = new AdminForm();
                    admin.Show();
                    this.Hide();
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
    }
}
