namespace Battle
{
    public partial class FormMain : Form
    {
        // Направление и скорость движения корабля по горизонтали.
        private int shipSpeed = 5;
        // Скорость полёта ядра вверх.
        private const int CannonBallSpeed = 9;

        public FormMain()
        {
            InitializeComponent();
            // Позволяет форме получать клавиши, даже если фокус у PictureBox.
            KeyPreview = true;
            Resize += FormMain_Resize;
        }

        /// <summary>Подготавливает игровое поле и начинает новую игру.</summary>
        private void FormMain_Shown(object sender, EventArgs e) => StartNewGame();

        /// <summary>Сбрасывает позиции объектов и запускает движение корабля.</summary>
        private void StartNewGame()
        {
            CenterCannonAtBottom();
            pictureBoxCanonBall.Visible = false;
            pictureBoxBoom.Visible = false;
            pictureBoxShip.Location = new Point(
                Math.Max(0, ClientSize.Width - pictureBoxShip.Width - 20), 20);
            pictureBoxShip.Visible = true;
            timerCanonBall.Stop();
            timerShip.Start();
        }

        /// <summary>Ставит пушку по центру нижнего края игрового окна.</summary>
        private void CenterCannonAtBottom()
        {
            pictureBoxCanon.Location = new Point(
                (ClientSize.Width - pictureBoxCanon.Width) / 2,
                ClientSize.Height - pictureBoxCanon.Height);
        }

        /// <summary>Повторно центрирует пушку при изменении размера окна.</summary>
        private void FormMain_Resize(object? sender, EventArgs e)
        {
            if (pictureBoxCanon != null)
                CenterCannonAtBottom();
        }

        /// <summary>Двигает корабль и разворачивает его у границ игрового поля.</summary>
        private void timerShip_Tick(object sender, EventArgs e)
        {
            int nextX = pictureBoxShip.Left + shipSpeed;
            if (nextX <= 0)
            {
                nextX = 0;
                shipSpeed = Math.Abs(shipSpeed);
            }
            else if (nextX + pictureBoxShip.Width >= ClientSize.Width)
            {
                nextX = ClientSize.Width - pictureBoxShip.Width;
                shipSpeed = -Math.Abs(shipSpeed);
            }
            pictureBoxShip.Left = nextX;
        }

        /// <summary>Обрабатывает пробел (выстрел) и Escape (выход).</summary>
        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }
            // Пока ядро летит, повторный выстрел запрещён.
            if (e.KeyCode == Keys.Space && !timerCanonBall.Enabled)
                FireCannon();
        }

        /// <summary>Создаёт одно ядро над стволом пушки и запускает его полёт.</summary>
        private void FireCannon()
        {
            pictureBoxCanonBall.Location = new Point(
                pictureBoxCanon.Left + (pictureBoxCanon.Width - pictureBoxCanonBall.Width) / 2,
                pictureBoxCanon.Top - pictureBoxCanonBall.Height);
            pictureBoxCanonBall.Visible = true;
            timerCanonBall.Start();
        }

        /// <summary>Передвигает ядро, проверяет попадание и завершение полёта.</summary>
        private void timerCanonBall_Tick(object sender, EventArgs e)
        {
            pictureBoxCanonBall.Top -= CannonBallSpeed;
            // Проверяем пересечение прямоугольников ядра и корабля, а не только одну точку.
            if (pictureBoxCanonBall.Bounds.IntersectsWith(pictureBoxShip.Bounds))
            {
                ShowHit();
                return;
            }
            // Если ядро полностью ушло за верхнюю границу, разрешаем новый выстрел.
            if (pictureBoxCanonBall.Bottom < 0)
            {
                pictureBoxCanonBall.Visible = false;
                timerCanonBall.Stop();
            }
        }

        /// <summary>Показывает взрыв при попадании и предлагает сыграть ещё раз.</summary>
        private void ShowHit()
        {
            timerCanonBall.Stop();
            timerShip.Stop();
            pictureBoxBoom.Location = new Point(
                pictureBoxShip.Left + (pictureBoxShip.Width - pictureBoxBoom.Width) / 2,
                pictureBoxShip.Top + (pictureBoxShip.Height - pictureBoxBoom.Height) / 2);
            pictureBoxShip.Visible = false;
            pictureBoxCanonBall.Visible = false;
            pictureBoxBoom.Visible = true;
            pictureBoxBoom.BringToFront();

            DialogResult result = MessageBox.Show(
                "Хотите сыграть ещё раз?", "Попадание!",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
                StartNewGame();
            else
                Close();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }
    }
}
