using System.Windows.Forms;

namespace SAS_admin_.teacher
{
    public partial class dash : Form
    {

        public dash()
        {
            InitializeComponent();
        }

        string teacherName, teacherId;
        public void add(string tName, string tId)
        {
            teacherName = tName;
            teacherId = tId;
            MessageBox.Show("Comming son with next version of software..,", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
