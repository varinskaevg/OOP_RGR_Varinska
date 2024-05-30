using System.Drawing.Drawing2D;

namespace VarinskaKyrsova;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
    }
    //Кнопка почати гру
    private void btnPlay_Click(object sender, EventArgs e)
    {
        GameForm gameForm = new GameForm();
        gameForm.FormClosed += GameForm_FormClosed;
        gameForm.Show();
        this.Hide();
    }
    //Закриття ції форми після відкриття нової
    private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        this.Close();
    }
    //Кнопка з інформацією про розробника
    private void button1_Click(object sender, EventArgs e)
    {
        MessageBox.Show("© Цю гру створила студентка групи 202-ТК \nВаринська Євгенія");
    }
}

