using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace AsyncFormDemo
{
    public partial class AsyncForm : Form
    {
        public AsyncForm()
        {
            InitializeComponent();

            this.cbxAutoLine.Checked = true;
            this.cbxAutoDate.Checked = true;

            var asyncFormHelper = AsyncUiHelper.Create(this.txtMessage, message => { this.txtMessage.AppendText(message); });
            FormHelper = asyncFormHelper;
        }

        public AsyncUiHelper FormHelper { get; set; }
        
        private void AsyncForm_Load(object sender, EventArgs e)
        {
        }
        
        private bool _processing = false;
        private int _messageIndex = 0;
        private void btnStart_Click(object sender, EventArgs e)
        {
            FormHelper.AutoAppendLine = this.cbxAutoLine.Checked;
            FormHelper.WithDatePrefix = this.cbxAutoDate.Checked;

            _messageIndex = 0;
            _processing = true;
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    if (!_processing)
                    {
                        break;
                    }
                    _messageIndex++;
                    Thread.Sleep(200);

                    FormHelper.SafeUpdateUi("message " + _messageIndex);
                }
                _processing = false;
            });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _processing = false;
            FormHelper.SafeUpdateUi("Stopped!");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtMessage.Clear();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var newForm = new AsyncLogForm())
            {
                newForm.StartPosition = FormStartPosition.CenterParent;
                newForm.ShowDialog(this);
            }
        }
    }
}
