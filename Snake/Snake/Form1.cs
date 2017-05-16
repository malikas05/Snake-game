using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        private int speed = 1;
        bool flag = false;

        public Form1()
        {
            InitializeComponent();

            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            StartGame();
        }

        private void StartGame()
        {
            new Settings();

            lblGameOver.Visible = false;
            lblNewGame.Visible = false;
            lblScore.Text = "0";
            speed = 1;
            labelSpeed.Text = speed.ToString();

            gameTimer.Interval = (int)Settings.Speed;
            Snake.Clear();
            Circle head = new Circle();
            head.X = 10;
            head.Y = 5;
            Snake.Add(head);

            GenerateFood();
        }

        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random rand = new Random();
            food.X = rand.Next(0, maxXPos);
            food.Y = rand.Next(0, maxYPos);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Input.KeyPressed(Keys.ShiftKey))
                flag = !flag;

            if (!flag)
            {
                if (Settings.GameOver)
                {
                    if (Input.KeyPressed(Keys.Enter))
                    {
                        StartGame();
                    }
                }
                else
                {
                    if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                        Settings.direction = Direction.Right;
                    else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                        Settings.direction = Direction.Left;
                    else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                        Settings.direction = Direction.Up;
                    else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                        Settings.direction = Direction.Down;
                    MovePlayer();

                }
            }
            if (Input.KeyPressed(Keys.Escape))
                Close();

            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                Brush snakeColour;

                for (int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                        snakeColour = Brushes.Green;
                    else
                        snakeColour = Brushes.Indigo;

                    canvas.FillRectangle(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                                      food.Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                }
            }
            else
            {
                lblGameOver.Text = "Game Over";
                lblGameOver.Visible = true;
                lblNewGame.Text = "Press Enter for New Game";
                lblNewGame.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    if (Snake[i].X < 0 || Snake[i].Y < 0
                        || Snake[i].X >= (pbCanvas.Size.Width / Settings.Width)
                        || Snake[i].Y >= (pbCanvas.Size.Height / Settings.Height))
                        Die();

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                            Snake[i].Y == Snake[j].Y)
                            Die();
                    }

                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                        Eat();
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Eat()
        {
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            labelSpeed.Text = speed.ToString();

            Snake.Add(circle);
            if (Settings.Speed > 90)
            {
                Settings.Speed -= 5;
                speed++;
            }
            else if (Settings.Speed >= 70)
            {
                Settings.Speed -= 3;
                speed++;
            }
            else if (Settings.Speed > 50)
            {
                Settings.Speed -= 2;
                speed++;
            }
            else
                Settings.Speed = 50;
            gameTimer.Interval = (int)(Settings.Speed);
            GenerateFood();

        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By Malikas 2016");
        }
    }
}
