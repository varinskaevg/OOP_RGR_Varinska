using System.Drawing.Drawing2D;

namespace VarinskaKyrsova;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
    }
    //������ ������ ���
    private void btnPlay_Click(object sender, EventArgs e)
    {
        GameForm gameForm = new GameForm();
        gameForm.FormClosed += GameForm_FormClosed;
        gameForm.Show();
        this.Hide();
    }
    //�������� ��� ����� ���� �������� ����
    private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        this.Close();
    }
    //������ � ����������� ��� ����������
    private void button1_Click(object sender, EventArgs e)
    {
        MessageBox.Show("� �� ��� �������� ��������� ����� 202-�� \n��������� ������");
    }
}

