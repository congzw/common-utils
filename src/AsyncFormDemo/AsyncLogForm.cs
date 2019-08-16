using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace AsyncFormDemo
{
    public partial class AsyncLogForm : Form
    {
        private readonly ISimpleLog _simpleLog = null;
        public AsyncLogForm()
        {
            InitializeComponent();

            var simpleLogFactory = SimpleLogFactory.Resolve();
            _simpleLog = simpleLogFactory.GetOrCreate("MockSomeLogger");

            this.cbxAutoLine.Checked = true;
            this.cbxAutoDate.Checked = true;

            MessageEventBusHelper = this.txtMessage.CreateAsyncUiHelperForMessageEventBus(message => { this.txtMessage.AppendText(message); });
        }
        
        public AsyncUiHelperForMessageEventBus MessageEventBusHelper { get; set; }

        private void AsyncLogForm_Load(object sender, EventArgs e)
        {

        }

        private bool _processing = false;
        private int _messageIndex = 0;
        private void btnStart_Click(object sender, EventArgs e)
        {
            MessageEventBusHelper.AutoAppendLine = this.cbxAutoLine.Checked;
            MessageEventBusHelper.WithDatePrefix = this.cbxAutoDate.Checked;

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

                    _simpleLog.LogInfo("message " + _messageIndex);
                }
                _processing = false;
            });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _processing = false;
            this.txtMessage.AppendText("Stopped!" +Environment.NewLine);
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
