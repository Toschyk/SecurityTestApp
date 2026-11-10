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

            string result = FileManager.Authenticate(username, password);

            if (result != null && result.StartsWith("LOCKED"))
            {
                string minutes = result.Split('|')[1];
                MessageBox.Show($"Аккаунт заблокирован! Попробуйте через {minutes} мин.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (result != null)
            {
                MessageBox.Show("Успешный вход!");
                if (result == "Admin")
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
