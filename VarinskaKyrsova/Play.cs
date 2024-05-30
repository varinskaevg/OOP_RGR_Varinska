using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VarinskaKyrsova
{
    //Конструктор який представляє форму гри "Морський бій"
    public partial class Play : Form
    {
        private DataGridView dgvPlayerField;
        private DataGridView dgvEmptyField;
        private List<Ship> ships;
        private List<Ship> savedShips = new List<Ship>();
        private List<Ship> botShips;
        private bool playerTurn = true;
        int hitsCount = 0;
        int missesCount = 0;
        private int playerDestroyedShips = 0;
        private int botDestroyedShips = 0;
        private Point? lastHit = null;
        private Random random = new Random();
        private List<Point> currentShipHits = new List<Point>();
        //Ініціалізація нового екземпляр класу Play
        internal Play(List<Ship> savedShips)
        {
            this.ships = savedShips;
            InitializeComponent();
            InitializeDataGridViews();
            RestoreSavedShips();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            dgvPlayerField.AllowUserToAddRows = false;
            dgvPlayerField.AllowUserToDeleteRows = false;
            dgvPlayerField.ReadOnly = true;

            dgvEmptyField.AllowUserToAddRows = false;
            dgvEmptyField.AllowUserToDeleteRows = false;
            dgvEmptyField.ReadOnly = true;
        }
        //Ініціалізація DataGridViews для гравця та противника.
        private void InitializeDataGridViews()
        {
            dgvPlayerField = new DataGridView();
            dgvPlayerField.Size = new Size(300, 300);
            dgvPlayerField.Location = new System.Drawing.Point((int)((this.ClientSize.Width - dgvPlayerField.Width) / 20), (int)((this.ClientSize.Height - dgvPlayerField.Height) / 4 + 20));
            dgvPlayerField.ColumnCount = 10;
            dgvPlayerField.RowCount = 11;
            InitializeDataGridViewStyles(dgvPlayerField);
            dgvEmptyField = new DataGridView();
            dgvEmptyField.Size = new Size(300, 300);
            dgvEmptyField.Location = new System.Drawing.Point(dgvPlayerField.Right + (int)((this.ClientSize.Width - dgvPlayerField.Width * 2) / 3 + 160), dgvPlayerField.Top);
            dgvEmptyField.ColumnCount = 10;
            dgvEmptyField.RowCount = 10;
            dgvEmptyField.CellClick += DgvEmptyField_CellClick;
            InitializeDataGridViewStyles(dgvEmptyField);
            this.Controls.Add(dgvPlayerField);
            this.Controls.Add(dgvEmptyField);
            PlaceBotShips();
        }
        //Налаштовання стилів для DataGridView.
        private void InitializeDataGridViewStyles(DataGridView dgv)
        {
            int cellWidth = dgv.Width / dgv.ColumnCount;
            int cellHeight = dgv.Height / dgv.RowCount;
            cellHeight += 3;
            dgv.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Red;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.BackgroundColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersVisible = false;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                dgv.Columns[i].Width = cellWidth;
            }

            for (int i = 0; i < dgv.RowCount; i++)
            {
                dgv.Rows[i].Height = cellHeight;
            }
            dgv.ScrollBars = ScrollBars.None;
            dgv.ReadOnly = true;
        }
        //Відтворення збережених кораблів гравця
        private void RestoreSavedShips()
        {
            foreach (var ship in ships)
            {
                int startX = ship.StartPosition.X;
                int startY = ship.StartPosition.Y;
                int size = ship.Size;
                bool vertical = ship.Vertical;
                Color backgroundColor = Color.Gray;
                for (int i = 0; i < size; i++)
                {
                    int x = vertical ? startX : startX;
                    int y = vertical ? startY : startY;
                    if (x >= 0 && x < dgvPlayerField.ColumnCount &&
                        y >= 0 && y < dgvPlayerField.RowCount)
                    {
                        dgvPlayerField[x, y].Tag = "ship";
                        dgvPlayerField[x, y].Style.BackColor = Color.Gray;
                    }
                }
            }
        }
        //Обробник подій кліків на полі противника.
        private void DgvEmptyField_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!playerTurn) return;

            int x = e.ColumnIndex;
            int y = e.RowIndex;

            if (x >= 0 && y >= 0 && dgvEmptyField.Rows[y].Cells[x].Value == null)
            {
                if (dgvEmptyField.Rows[y].Cells[x].Tag != null && dgvEmptyField.Rows[y].Cells[x].Tag.ToString() == "ship")
                {
                    dgvEmptyField.Rows[y].Cells[x].Value = "✖";
                    dgvEmptyField.Rows[y].Cells[x].Style.ForeColor = Color.Black;
                    hitsCount++;

                    foreach (var ship in botShips)
                    {
                        if (IsShipDestroyed(ship, dgvEmptyField))
                        {
                            playerDestroyedShips++;
                            MarkSurroundingCells(ship, dgvEmptyField);
                        }
                    }
                }
                else
                {
                    dgvEmptyField.Rows[y].Cells[x].Value = "•";
                    dgvEmptyField.Rows[y].Cells[x].Style.ForeColor = Color.Red;
                    missesCount++;
                }

                dgvEmptyField.Rows[y].Cells[x].ReadOnly = true;
                playerTurn = false;

                CheckForVictory();

                if (!playerTurn)
                {
                    Task.Run(async () => await BotTurn());
                }

                label45.Text = "Ваш хід";
                label45.ForeColor = Color.Green;
                label45.BorderStyle = BorderStyle.FixedSingle;

                label43.Text = $"Попадання: {hitsCount}";
                label44.Text = $"Промахи: {missesCount}";
            }
        }
        //Обробник подій противника.
        private async Task BotTurn()
        {
            int x, y;

            if (lastHit == null)
            {
                do
                {
                    x = random.Next(0, dgvPlayerField.ColumnCount);
                    y = random.Next(0, dgvPlayerField.RowCount);
                } while (dgvPlayerField.Rows[y].Cells[x].Tag != null && dgvPlayerField.Rows[y].Cells[x].Tag.ToString() == "shot");
            }
            else
            {
                (x, y) = TryToFinishShip(dgvPlayerField);
            }

            if (dgvPlayerField.Rows[y].Cells[x].Tag != null && dgvPlayerField.Rows[y].Cells[x].Tag.ToString() == "ship")
            {
                dgvPlayerField.Rows[y].Cells[x].Value = "✖";
                dgvPlayerField.Rows[y].Cells[x].Style.ForeColor = Color.Black;
                dgvPlayerField.Rows[y].Cells[x].Tag = "shot";
                dgvPlayerField.Rows[y].Cells[x].ReadOnly = true;

                lastHit = new Point(x, y);
                currentShipHits.Add(lastHit);

                bool shipDestroyed = false;
                foreach (var ship in ships)
                {
                    if (IsShipDestroyed(ship, dgvPlayerField))
                    {
                        botDestroyedShips++;
                        MarkSurroundingCells(ship, dgvPlayerField);
                        shipDestroyed = true;
                    }
                }

                if (!shipDestroyed)
                {
                    lastHit = new Point(x, y);
                }
            }
            else
            {
                dgvPlayerField.Rows[y].Cells[x].Value = "•";
                dgvPlayerField.Rows[y].Cells[x].Style.ForeColor = Color.Red;
                dgvPlayerField.Rows[y].Cells[x].Tag = "shot";
                dgvPlayerField.Rows[y].Cells[x].ReadOnly = true;
                lastHit = null;
            }

            await Task.Delay(1000);
            playerTurn = true;
            label45.Text = "Хід противника";
            label45.ForeColor = Color.Red;
            label45.BorderStyle = BorderStyle.FixedSingle;

            CheckForVictory();
        }
        //Метод противника для добивання кораблів
        private (int, int) TryToFinishShip(DataGridView dgvPlayerField)
        {
            List<(int, int)> directions = new List<(int, int)>
            {
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            };

            foreach (var hit in currentShipHits)
            {
                foreach (var (dx, dy) in directions)
                {
                    int finishX = hit.X + dx;
                    int finishY = hit.Y + dy;

                    if (finishX >= 0 && finishX < dgvPlayerField.ColumnCount &&
                        finishY >= 0 && finishY < dgvPlayerField.RowCount &&
                        dgvPlayerField.Rows[finishY].Cells[finishX].Tag != null &&
                        dgvPlayerField.Rows[finishY].Cells[finishX].Tag.ToString() == "ship")
                    {
                        return (finishX, finishY);
                    }
                }
            }

            foreach (var hit in currentShipHits)
            {
                foreach (var (dx, dy) in directions)
                {
                    int adjX = hit.X + dx;
                    int adjY = hit.Y + dy;

                    if (adjX >= 0 && adjX < dgvPlayerField.ColumnCount &&
                        adjY >= 0 && adjY < dgvPlayerField.RowCount &&
                        (dgvPlayerField.Rows[adjY].Cells[adjX].Tag == null || dgvPlayerField.Rows[adjY].Cells[adjX].Tag.ToString() != "shot"))
                    {
                        return (adjX, adjY);
                    }
                }
            }

            int shotX, shotY;
            do
            {
                shotX = random.Next(0, dgvPlayerField.ColumnCount);
                shotY = random.Next(0, dgvPlayerField.RowCount);
            } while (dgvPlayerField.Rows[shotY].Cells[shotX].Tag != null && dgvPlayerField.Rows[shotY].Cells[shotX].Tag.ToString() == "shot");

            return (shotX, shotY);
        }
        //Маркерує точки навколо потоплених кораблів
        private void MarkSurroundingCells(Ship ship, DataGridView dgvField)
        {
            int startX = ship.StartPosition.X;
            int startY = ship.StartPosition.Y;
            bool vertical = ship.Vertical;

            for (int i = 0; i < ship.Size; i++)
            {
                int x = vertical ? startX : startX + i;
                int y = vertical ? startY + i : startY;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && ny >= 0 && nx < dgvField.ColumnCount && ny < dgvField.RowCount)
                        {
                            if (dgvField.Rows[ny].Cells[nx].Value == null || (dgvField.Rows[ny].Cells[nx].Value.ToString() != "✖" && dgvField.Rows[ny].Cells[nx].Value.ToString() != "•"))
                            {
                                dgvField.Rows[ny].Cells[nx].Value = "•";
                                dgvField.Rows[ny].Cells[nx].Style.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }
        // Перевірка чи корабель знищенний
        private bool IsShipDestroyed(Ship ship, DataGridView dgvField)
        {
            bool destroyed = true;
            int startX = ship.StartPosition.X;
            int startY = ship.StartPosition.Y;

            for (int i = 0; i < ship.Size; i++)
            {
                int shipX = ship.Vertical ? startX : startX + i;
                int shipY = ship.Vertical ? startY + i : startY;

                if (dgvField.Rows[shipY].Cells[shipX].Value == null || dgvField.Rows[shipY].Cells[shipX].Value.ToString() != "✖")
                {
                    destroyed = false;
                    break;
                }
            }

            if (destroyed)
            {
                MarkSurroundingCells(ship, dgvField);
            }

            return destroyed;
        }
        //Фукнція розстановки кораблів противника
        private void PlaceBotShips()
        {
            Random random = new Random();
            List<Ship> botShips = new List<Ship>();

            int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

            foreach (int shipSize in shipSizes)
            {
                bool shipPlaced = false;

                while (!shipPlaced)
                {
                    bool isVertical = random.Next(2) == 0;
                    int startX = random.Next(0, dgvEmptyField.ColumnCount);
                    int startY = random.Next(0, dgvEmptyField.RowCount);
                    if (CanPlaceShip(startX, startY, shipSize, isVertical, botShips))
                    {
                        botShips.Add(new Ship(startX, startY, shipSize, isVertical));
                        for (int i = 0; i < shipSize; i++)
                        {
                            int x = isVertical ? startX : startX + i;
                            int y = isVertical ? startY + i : startY;
                            if (x >= 0 && x < dgvEmptyField.ColumnCount && y >= 0 && y < dgvEmptyField.RowCount)
                            {
                                dgvEmptyField.Rows[y].Cells[x].Tag = "ship";
                                dgvEmptyField.Rows[y].Cells[x].Style.BackColor = Color.White;
                            }
                            else
                            {
                                MessageBox.Show($"Invalid cell coordinates: {x}, {y}");
                            }
                        }

                        shipPlaced = true;
                    }
                }
            }
            dgvEmptyField.Refresh();
            this.botShips = botShips;
        }
        //Перевірка можливості розміщення корабля на полі
        private bool CanPlaceShip(int startX, int startY, int shipSize, bool isVertical, List<Ship> ships)
        {
            if (isVertical && startY + shipSize > dgvEmptyField.RowCount)
                return false;
            if (!isVertical && startX + shipSize > dgvEmptyField.ColumnCount)
                return false;

            foreach (Ship ship in ships)
            {
                if (ship.Vertical == isVertical)
                {

                    if (startX > 0 && dgvEmptyField.Rows[startY].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startX + 1 < dgvEmptyField.ColumnCount && dgvEmptyField.Rows[startY].Cells[startX + 1].Tag != null && dgvEmptyField.Rows[startY].Cells[startX + 1].Tag.ToString() == "ship")
                        return false;

                    for (int i = startY; i < startY + shipSize; i++)
                    {
                        if (i >= 0 && i < dgvEmptyField.RowCount)
                        {
                            if (startX > 0 && dgvEmptyField.Rows[i].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[i].Cells[startX - 1].Tag.ToString() == "ship")
                                return false;
                            if (startX + 1 < dgvEmptyField.ColumnCount && dgvEmptyField.Rows[i].Cells[startX + 1].Tag != null && dgvEmptyField.Rows[i].Cells[startX + 1].Tag.ToString() == "ship")
                                return false;
                        }
                    }

                    if (startY > 0 && dgvEmptyField.Rows[startY - 1].Cells[startX].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX].Tag.ToString() == "ship")
                        return false;
                    if (startY + shipSize < dgvEmptyField.RowCount && dgvEmptyField.Rows[startY + shipSize].Cells[startX].Tag != null && dgvEmptyField.Rows[startY + shipSize].Cells[startX].Tag.ToString() == "ship")
                        return false;
                }
                else
                {
                    if (startY > 0 && dgvEmptyField.Rows[startY - 1].Cells[startX].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX].Tag.ToString() == "ship")
                        return false;
                    if (startY + 1 < dgvEmptyField.RowCount && dgvEmptyField.Rows[startY + 1].Cells[startX].Tag != null && dgvEmptyField.Rows[startY + 1].Cells[startX].Tag.ToString() == "ship")
                        return false;

                    for (int i = startX; i < startX + shipSize; i++)
                    {
                        if (i >= 0 && i < dgvEmptyField.ColumnCount)
                        {
                            if (startY > 0 && dgvEmptyField.Rows[startY - 1].Cells[i].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[i].Tag.ToString() == "ship")
                                return false;
                            if (startY + 1 < dgvEmptyField.RowCount && dgvEmptyField.Rows[startY + 1].Cells[i].Tag != null && dgvEmptyField.Rows[startY + 1].Cells[i].Tag.ToString() == "ship")
                                return false;
                        }
                    }

                    if (startX > 0 && dgvEmptyField.Rows[startY].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startX + shipSize < dgvEmptyField.ColumnCount && dgvEmptyField.Rows[startY].Cells[startX + shipSize].Tag != null && dgvEmptyField.Rows[startY].Cells[startX + shipSize].Tag.ToString() == "ship")
                        return false;
                }

                if (isVertical)
                {
                    if (startX > 0 && startY > 0 && dgvEmptyField.Rows[startY - 1].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startX > 0 && startY + shipSize < dgvEmptyField.RowCount && dgvEmptyField.Rows[startY + shipSize].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY + shipSize].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startX + 1 < dgvEmptyField.ColumnCount && startY > 0 && dgvEmptyField.Rows[startY - 1].Cells[startX + 1].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX + 1].Tag.ToString() == "ship")
                        return false;
                    if (startX + 1 < dgvEmptyField.ColumnCount && startY + shipSize < dgvEmptyField.RowCount && dgvEmptyField.Rows[startY + shipSize].Cells[startX + 1].Tag != null && dgvEmptyField.Rows[startY + shipSize].Cells[startX + 1].Tag.ToString() == "ship")
                        return false;
                }
                else
                {
                    if (startY > 0 && startX > 0 && dgvEmptyField.Rows[startY - 1].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startY > 0 && startX + shipSize < dgvEmptyField.ColumnCount && dgvEmptyField.Rows[startY - 1].Cells[startX + shipSize].Tag != null && dgvEmptyField.Rows[startY - 1].Cells[startX + shipSize].Tag.ToString() == "ship")
                        return false;
                    if (startY + 1 < dgvEmptyField.RowCount && startX > 0 && dgvEmptyField.Rows[startY + 1].Cells[startX - 1].Tag != null && dgvEmptyField.Rows[startY + 1].Cells[startX - 1].Tag.ToString() == "ship")
                        return false;
                    if (startY + 1 < dgvEmptyField.RowCount && startX + shipSize < dgvEmptyField.ColumnCount && dgvEmptyField.Rows[startY + 1].Cells[startX + shipSize].Tag != null && dgvEmptyField.Rows[startY + 1].Cells[startX + shipSize].Tag.ToString() == "ship")
                        return false;
                }
            }

            return true;
        }
        //Перевірка, чи всі кораблі знищені.
        private bool IsAllShipsDestroyed(List<Ship> ships, DataGridView dgvField)
        {
            foreach (Ship ship in ships)
            {
                if (!IsShipDestroyed(ship, dgvField))
                    return false;
            }
            return true;
        }
        //Перевірка, чи відбулася перемога одного з гравців.
        private void CheckForVictory()
        {
            if (IsAllShipsDestroyed(botShips, dgvEmptyField))
            {
                MessageBox.Show("Ви перемогли!", "Гра завершена");
                EndGame();
            }
            else if (IsAllShipsDestroyed(ships, dgvPlayerField))
            {
                MessageBox.Show("Ви програли!", "Гра завершена");
                EndGame();
            }
        }
        //Завершення гри
        private void EndGame()
        {
            dgvEmptyField.CellClick -= DgvEmptyField_CellClick;
            playerTurn = false;
        }

    }
}