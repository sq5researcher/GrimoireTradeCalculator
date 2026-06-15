using System.Text;

namespace GrimoireTradeCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            var calculator = new Calculator();
            var results = calculator.GetEquivalentCombinations(
                new[]
                {
                    (int)numericUpDown1.Value,
                    (int)numericUpDown2.Value,
                    (int)numericUpDown3.Value,
                    (int)numericUpDown4.Value,
                    (int)numericUpDown5.Value,
                    (int)numericUpDown6.Value,
                    (int)numericUpDown7.Value,
                    (int)numericUpDown8.Value,
                    (int)numericUpDown9.Value,
                    (int)numericUpDown10.Value,
                },
                (int)numericUpDown11.Value,
                comboBox1.SelectedItem?.ToString() ?? string.Empty);

            if (results.Count == 0)
            {
                richTextBox1.Text = "トレードできません";
                return;
            }

            StringBuilder builder = new StringBuilder();
            foreach (var result in results)
            {
                string counts = string.Join(", ", result.Counts);
                builder.Append($"[{counts}]");
                builder.AppendLine();
            }
            richTextBox1.Text = builder.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            numericUpDown3.Value = 0;
            numericUpDown4.Value = 0;
            numericUpDown5.Value = 0;
            numericUpDown6.Value = 0;
            numericUpDown7.Value = 0;
            numericUpDown8.Value = 0;
            numericUpDown9.Value = 0;
            numericUpDown10.Value = 0;
        }
    }
}
