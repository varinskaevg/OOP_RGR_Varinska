using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VarinskaKyrsova
{
    //Конструктор який представляє головну форму гри "Морський бій".
    public partial class GameForm : Form
    {
        public DataGridView dgvPlayerField;
        private int placedOneDeckShips = 0;
        private int placedTwoDeckShips = 0;
        private int placedThreeDeckShips = 0;
        private int placedFourDeckShips = 0;
        private bool radioOneEnabled = true;
        private bool radioTwoEnabled = true;
        private bool radioThreeEnabled = true;
        private bool radioFourEnabled = true;
        private bool isVertical = false;
        private List<Ship> ships;
        private List<Ship> savedShips;
        private bool isPlacementSaved = false;
        //Конструктор для ініціалізації форми гри.
        public GameForm()
        {
            InitializeComponent();
            InitializeDataGridViews();
            ships = new List<Ship>();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        // Ініціалізує компонент DataGridView для поля гравця.
        private void InitializeDataGridViews()
        {
            dgvPlayerField = new DataGridView();
            dgvPlayerField.Size = new Size(300, 300);
            dgvPlayerField.Location = new System.Drawing.Point((int)((this.ClientSize.Width - dgvPlayerField.Width) / 20), (int)((this.ClientSize.Height - dgvPlayerField.Height) / 6));
            dgvPlayerField.ColumnCount = 10;
            dgvPlayerField.RowCount = 11;
            int cellWidth = dgvPlayerField.Width / dgvPlayerField.ColumnCount;
            int cellHeight = dgvPlayerField.Height / dgvPlayerField.RowCount;
            cellHeight += 3;
            dgvPlayerField.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgvPlayerField.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvPlayerField.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvPlayerField.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvPlayerField.BackgroundColor = Color.White;
            dgvPlayerField.RowHeadersVisible = false;
            dgvPlayerField.ColumnHeadersVisible = false;
            dgvPlayerField.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPlayerField.MultiSelect = false;
            dgvPlayerField.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvPlayerField.AllowUserToAddRows = false;
            dgvPlayerField.AllowUserToDeleteRows = false;
            dgvPlayerField.AllowUserToResizeColumns = false;
            dgvPlayerField.AllowUserToResizeRows = false;
            dgvPlayerField.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvPlayerField.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            for (int i = 0; i < dgvPlayerField.ColumnCount; i++)
            {
                dgvPlayerField.Columns[i].Width = cellWidth;
            }

            for (int i = 0; i < dgvPlayerField.RowCount; i++)
            {
                dgvPlayerField.Rows[i].Height = cellHeight;
            }
            dgvPlayerField.ReadOnly = true;
            dgvPlayerField.ScrollBars = ScrollBars.None;
            this.Controls.Add(dgvPlayerField);
            radioOne.CheckedChanged += RadioOne_CheckedChanged;
            radioTwo.CheckedChanged += RadioTwo_CheckedChanged;
            radioThree.CheckedChanged += RadioThree_CheckedChanged;
            radioFour.CheckedChanged += RadioFour_CheckedChanged;
            radioVer.CheckedChanged += RadioVer_CheckedChanged;
            radioGor.CheckedChanged += RadioGor_CheckedChanged;
            ships = new List<Ship>();
        }
        //Обробник події для зміни стану радіокнопки однопалубного корабля.
        private void RadioOne_CheckedChanged(object sender, EventArgs e)
        {
            if (radioOne.Checked)
            {
                dgvPlayerField.CellClick += DgvPlayerField_CellClickForOneDeckShipPlacement;
            }
            else
            {
                dgvPlayerField.CellClick -= DgvPlayerField_CellClickForOneDeckShipPlacement;
            }
        }
        //Обробник події для зміни стану радіокнопки двопалубного корабля.
        private void RadioTwo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTwo.Checked)
            {
                dgvPlayerField.CellClick += DgvPlayerField_CellClickForTwoDeckShipPlacement;
            }
            else
            {
                dgvPlayerField.CellClick -= DgvPlayerField_CellClickForTwoDeckShipPlacement;
            }
        }
        //Обробник події для зміни стану радіокнопки трьохпалубного корабля.
        private void RadioThree_CheckedChanged(object sender, EventArgs e)
        {
            if (radioThree.Checked)
            {
                dgvPlayerField.CellClick += DgvPlayerField_CellClickForThreeDeckShipPlacement;
            }
            else
            {
                dgvPlayerField.CellClick -= DgvPlayerField_CellClickForThreeDeckShipPlacement;
            }
        }
        //Обробник події для зміни стану радіокнопки чотирьохпалубного корабля.
        private void RadioFour_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFour.Checked)
            {
                dgvPlayerField.CellClick += DgvPlayerField_CellClickForFourDeckShipPlacement;
            }
            else
            {
                dgvPlayerField.CellClick -= DgvPlayerField_CellClickForFourDeckShipPlacement;
            }
        }
        //Обробник події для зміни стану радіокнопки вертикального положення.
        private void RadioVer_CheckedChanged(object sender, EventArgs e)
        {
            if (radioVer.Checked)
            {
                isVertical = true;
            }
        }
        //Обробник події для зміни стану радіокнопки горизонтального положення.
        private void RadioGor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioGor.Checked)
            {
                isVertical = false;
            }
        }
        //Обробник події для розміщення однопалубного корабля.
        private void DgvPlayerField_CellClickForOneDeckShipPlacement(object sender, DataGridViewCellEventArgs e)
        {
            if (placedOneDeckShips < 4)
            {
                DataGridViewCell cell = dgvPlayerField[e.ColumnIndex, e.RowIndex];
                if (cell.Tag == null)
                {
                    cell.Tag = "ship";
                    cell.Value = "✖";
                    MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 1);
                    placedOneDeckShips++;
                    if (placedOneDeckShips >= 4)
                    {
                        radioOne.Enabled = false;
                        radioOneEnabled = false;
                    }
                }
            }
        }
        //Обробник події для розміщення двопалубного корабля.
        private void DgvPlayerField_CellClickForTwoDeckShipPlacement(object sender, DataGridViewCellEventArgs e)
        {
            if (placedTwoDeckShips < 3)
            {
                DataGridViewCell cell = dgvPlayerField[e.ColumnIndex, e.RowIndex];
                if (cell.Tag == null)
                {
                    if (isVertical)
                    {
                        if (e.RowIndex < dgvPlayerField.RowCount - 1 &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 1].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell = dgvPlayerField[e.ColumnIndex, e.RowIndex + 1];
                            nextCell.Tag = "ship";
                            nextCell.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 2);
                            placedTwoDeckShips++;
                            if (placedTwoDeckShips >= 3)
                            {
                                radioTwo.Enabled = false;
                                radioTwoEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (e.ColumnIndex < dgvPlayerField.ColumnCount - 1 &&
                            dgvPlayerField[e.ColumnIndex + 1, e.RowIndex].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell = dgvPlayerField[e.ColumnIndex + 1, e.RowIndex];
                            nextCell.Tag = "ship";
                            nextCell.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 2);
                            placedTwoDeckShips++;
                            if (placedTwoDeckShips >= 3)
                            {
                                radioTwo.Enabled = false;
                                radioTwoEnabled = false;
                            }
                        }
                    }
                }
            }
        }
        //Обробник події для розміщення трипалубного корабля.
        private void DgvPlayerField_CellClickForThreeDeckShipPlacement(object sender, DataGridViewCellEventArgs e)
        {
            if (placedThreeDeckShips < 2)
            {
                DataGridViewCell cell = dgvPlayerField[e.ColumnIndex, e.RowIndex];
                if (cell.Tag == null)
                {
                    if (isVertical)
                    {
                        if (e.RowIndex < dgvPlayerField.RowCount - 2 &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 1].Tag == null &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 2].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell1 = dgvPlayerField[e.ColumnIndex, e.RowIndex + 1];
                            nextCell1.Tag = "ship";
                            nextCell1.Value = "✖";
                            DataGridViewCell nextCell2 = dgvPlayerField[e.ColumnIndex, e.RowIndex + 2];
                            nextCell2.Tag = "ship";
                            nextCell2.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 3);
                            placedThreeDeckShips++;
                            if (placedThreeDeckShips >= 2)
                            {
                                radioThree.Enabled = false;
                                radioThreeEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (e.ColumnIndex < dgvPlayerField.ColumnCount - 2 &&
                            dgvPlayerField[e.ColumnIndex + 1, e.RowIndex].Tag == null &&
                            dgvPlayerField[e.ColumnIndex + 2, e.RowIndex].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell1 = dgvPlayerField[e.ColumnIndex + 1, e.RowIndex];
                            nextCell1.Tag = "ship";
                            nextCell1.Value = "✖";
                            DataGridViewCell nextCell2 = dgvPlayerField[e.ColumnIndex + 2, e.RowIndex];
                            nextCell2.Tag = "ship";
                            nextCell2.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 3);
                            placedThreeDeckShips++;
                            if (placedThreeDeckShips >= 2)
                            {
                                radioThree.Enabled = false;
                                radioThreeEnabled = false;
                            }
                        }
                    }
                }
            }
        }
        //Обробник події для розміщення чотирьохпалубного корабля.
        private void DgvPlayerField_CellClickForFourDeckShipPlacement(object sender, DataGridViewCellEventArgs e)
        {
            if (placedFourDeckShips < 1)
            {
                DataGridViewCell cell = dgvPlayerField[e.ColumnIndex, e.RowIndex];
                if (cell.Tag == null)
                {
                    if (isVertical)
                    {
                        if (e.RowIndex < dgvPlayerField.RowCount - 3 &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 1].Tag == null &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 2].Tag == null &&
                            dgvPlayerField[e.ColumnIndex, e.RowIndex + 3].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell1 = dgvPlayerField[e.ColumnIndex, e.RowIndex + 1];
                            nextCell1.Tag = "ship";
                            nextCell1.Value = "✖";
                            DataGridViewCell nextCell2 = dgvPlayerField[e.ColumnIndex, e.RowIndex + 2];
                            nextCell2.Tag = "ship";
                            nextCell2.Value = "✖";
                            DataGridViewCell nextCell3 = dgvPlayerField[e.ColumnIndex, e.RowIndex + 3];
                            nextCell3.Tag = "ship";
                            nextCell3.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 4);
                            placedFourDeckShips++;
                            if (placedFourDeckShips >= 1)
                            {
                                radioFour.Enabled = false;
                                radioFourEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (e.ColumnIndex < dgvPlayerField.ColumnCount - 3 &&
                            dgvPlayerField[e.ColumnIndex + 1, e.RowIndex].Tag == null &&
                            dgvPlayerField[e.ColumnIndex + 2, e.RowIndex].Tag == null &&
                            dgvPlayerField[e.ColumnIndex + 3, e.RowIndex].Tag == null)
                        {
                            cell.Tag = "ship";
                            cell.Value = "✖";
                            DataGridViewCell nextCell1 = dgvPlayerField[e.ColumnIndex + 1, e.RowIndex];
                            nextCell1.Tag = "ship";
                            nextCell1.Value = "✖";
                            DataGridViewCell nextCell2 = dgvPlayerField[e.ColumnIndex + 2, e.RowIndex];
                            nextCell2.Tag = "ship";
                            nextCell2.Value = "✖";
                            DataGridViewCell nextCell3 = dgvPlayerField[e.ColumnIndex + 3, e.RowIndex];
                            nextCell3.Tag = "ship";
                            nextCell3.Value = "✖";
                            MarkSurroundingCells(e.ColumnIndex, e.RowIndex, 4);
                            placedFourDeckShips++;
                            if (placedFourDeckShips >= 1)
                            {
                                radioFour.Enabled = false;
                                radioFourEnabled = false;
                            }
                        }
                    }
                }
            }
        }
        //Ця функція маркує навколишні комірки як заборонені для розміщення кораблів
        private void MarkSurroundingCells(int columnIndex, int rowIndex, int shipSize)
        {
            int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dy = { 0, -1, 0, 1, -1, 1, -1, 1 };

            for (int i = 0; i < dx.Length; i++)
            {
                for (int j = 0; j < shipSize; j++)
                {
                    int newX = columnIndex + (isVertical ? 0 : j);
                    int newY = rowIndex + (isVertical ? j : 0);
                    int surroundX = newX + dx[i];
                    int surroundY = newY + dy[i];

                    if (surroundX >= 0 && surroundX < dgvPlayerField.ColumnCount && surroundY >= 0 && surroundY < dgvPlayerField.RowCount)
                    {
                        DataGridViewCell cell = dgvPlayerField[surroundX, surroundY];
                        if (cell.Tag == null)
                        {
                            cell.Style.BackColor = Color.Red;
                            cell.Tag = "forbidden";
                        }
                    }
                }
            }
        }
        //Очистка поля гравця
        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            ClearPlayerField();
        }
        //Ця функція виконує повне очищення поля гравця
        private void ClearPlayerField()
        {
            dgvPlayerField.Rows.Clear();
            dgvPlayerField.Columns.Clear();
            this.Controls.Remove(dgvPlayerField);
            dgvPlayerField.Dispose();
            InitializeDataGridViews();
            this.Controls.Add(dgvPlayerField);
            ResetShipPlacement();
        }
        //Ця функція скидає розміщення кораблів на початковий стан
        private void ResetShipPlacement()
        {
            placedOneDeckShips = 0;
            placedTwoDeckShips = 0;
            placedThreeDeckShips = 0;
            placedFourDeckShips = 0;
            radioOne.Checked = false;
            radioTwo.Checked = false;
            radioThree.Checked = false;
            radioFour.Checked = false;
            radioOne.Enabled = true;
            radioTwo.Enabled = true;
            radioThree.Enabled = true;
            radioFour.Enabled = true;
        }
        //Ця функція зберігає поточне розміщення кораблів
        private void SaveShipPlacement()
        {
            List<Ship> currentShips = new List<Ship>();

            foreach (DataGridViewRow row in dgvPlayerField.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Tag != null && cell.Tag.ToString() == "ship")
                    {
                        int startX = cell.ColumnIndex;
                        int startY = cell.RowIndex;
                        bool vertical = false;
                        int size = 1;
                        if (startX < dgvPlayerField.ColumnCount - 1 && dgvPlayerField[startX + 1, startY].Tag != null && dgvPlayerField[startX + 1, startY].Tag.ToString() == "ship")
                        {
                            size = 2;
                            vertical = false;
                        }
                        else if (startY < dgvPlayerField.RowCount - 1 && dgvPlayerField[startX, startY + 1].Tag != null && dgvPlayerField[startX, startY + 1].Tag.ToString() == "ship")
                        {
                            size = 2;
                            vertical = true;
                        }

                        for (int i = 2; i < 4; i++)
                        {
                            if (!vertical && startX < dgvPlayerField.ColumnCount - i && dgvPlayerField[startX + i, startY].Tag != null && dgvPlayerField[startX + i, startY].Tag.ToString() == "ship")
                            {
                                size = i + 1;
                            }
                            else if (vertical && startY < dgvPlayerField.RowCount - i && dgvPlayerField[startX, startY + i].Tag != null && dgvPlayerField[startX, startY + i].Tag.ToString() == "ship")
                            {
                                size = i + 1;
                            }
                        }

                        currentShips.Add(new Ship(startX, startY, size, vertical));
                    }
                }
            }
            this.savedShips = currentShips;
        }
        //Ця функція перевіряє, чи всі кораблі розміщені перед збереженням і, у випадку успіху, зберігає розміщення.
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (placedOneDeckShips == 4 && placedTwoDeckShips == 3 && placedThreeDeckShips == 2 && placedFourDeckShips == 1)
            {
                SaveShipPlacement();
                MessageBox.Show("Расположение кораблей сохранено.");
                isPlacementSaved = true;
            }
            else
            {
                MessageBox.Show("Расставьте все корабли перед сохранением.");
            }
        }
        //Ця функція перевіряє чи збережена розстановка кораблів, і переходить до гри
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (isPlacementSaved)
            {
                Play playForm = new Play(savedShips);
                playForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Збережіть розстановку перед продовженням.");
            }
        }
    }
}