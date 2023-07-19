using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace ADuserLastChangePassword
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtSearch.KeyDown += txtSearch_KeyDown;

            Domain domain = Domain.GetCurrentDomain();
            DomainName.Text = domain.Name;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Prevent the Enter key from making a sound by handling the KeyDown event
                e.SuppressKeyPress = true;
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            try
            {
                output.Clear(); // Clear previous search results

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    using (UserPrincipal userPrincipal = new UserPrincipal(context))
                    {
                        // Set the SamAccountName and DisplayName properties to search for a partial match
                        userPrincipal.SamAccountName = "*" + txtSearch.Text + "*";

                        using (PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal))
                        {
                            foreach (UserPrincipal user in searcher.FindAll())
                            {
                                // Retrieve user information
                                string fullName = user.DisplayName;
                                string logonName = user.SamAccountName;
                                DateTime? lastPasswordChange = user.LastPasswordSet;

                                // Add the user information to the ListBox with the last password change details
                                output.AppendText($"Full Name: {fullName}, Logon Name: {logonName}, Last Password Change: {lastPasswordChange}\n\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the AD query
                MessageBox.Show($"There is nothing to return", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Run Program as admin and make sure this computer is connected to a AD \n\n SAMAccountName or Logon Name is what a user signs into computers with like john.deer or j.deer depending on what you are using.\n\n" +
                $" This program with auto connect to your active directory if you are a admin and search for a users name and will populate when user has last changed thier password." 
                , "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
