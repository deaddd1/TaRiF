using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TariffSelector
{
    // Модель тарифного плану
    public class Tariff
    {
        public string Name { get; set; } = string.Empty;
        public int InternetGb { get; set; }
        public int Minutes { get; set; }
        public int Sms { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class MainForm : Form
    {
        // 4 зашитих у код тарифи
        private readonly List<Tariff> _tariffs = new()
        {
            new Tariff {
                Name = "Базовий Економ",
                InternetGb = 5,
                Minutes = 150,
                Sms = 50,
                Price = 120,
                Description = "Для тих, кому потрібен інтернет лише для месенджерів та базові дзвінки."
            },
            new Tariff {
                Name = "Тільки Дзвінки",
                InternetGb = 1,
                Minutes = 800,
                Sms = 100,
                Price = 150,
                Description = "Багато розмов і мінімум інтернету для класичного спілкування."
            },
            new Tariff {
                Name = "Комфортний Оптимум",
                InternetGb = 25,
                Minutes = 500,
                Sms = 150,
                Price = 220,
                Description = "Збалансований варіант для відео, музики та регулярних дзвінків."
            },
            new Tariff {
                Name = "Безліміт Макс Pro",
                InternetGb = 100, // умовний безліміт
                Minutes = 1500,
                Sms = 300,
                Price = 350,
                Description = "Максимальний пакет для активного використання без обмежень."
            }
        };

        private NumericUpDown _numGb = null!;
        private NumericUpDown _numMinutes = null!;
        private NumericUpDown _numSms = null!;
        private Button _btnCalculate = null!;
        private Label _lblResultTitle = null!;
        private Label _lblResultPrice = null!;
        private Label _lblResultDetails = null!;
        private Panel _resultPanel = null!;

        public MainForm()
        {
            InitializeComponentLayout();
        }

        private void InitializeComponentLayout()
        {
            Text = "Підбір тарифного плану // Tariff Selector";
            Width = 560;
            Height = 580;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 9.5f);
            BackColor = Color.FromArgb(248, 250, 252);

            // Заголовок
            var lblHeader = new Label
            {
                Text = "Розрахунок ідеального тарифу",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = "Вкажіть ваші орієнтовні потреби на місяць:",
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(27, 52),
                AutoSize = true
            };

            // Блок введення
            var inputGroup = new GroupBox
            {
                Text = "Ваші потреби на місяць",
                Location = new Point(25, 80),
                Size = new Size(495, 170),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            // 1. Інтернет
            var lblGb = new Label { Text = "Інтернет (ГБ):", Location = new Point(20, 35), AutoSize = true, ForeColor = Color.Black };
            _numGb = new NumericUpDown
            {
                Location = new Point(20, 58),
                Width = 130,
                Maximum = 200,
                Value = 10
            };

            // 2. Дзвінки
            var lblMin = new Label { Text = "Дзвінки (хвилин):", Location = new Point(180, 35), AutoSize = true, ForeColor = Color.Black };
            _numMinutes = new NumericUpDown
            {
                Location = new Point(180, 58),
                Width = 130,
                Maximum = 5000,
                Value = 200
            };

            // 3. SMS
            var lblSms = new Label { Text = "SMS повідомлення:", Location = new Point(340, 35), AutoSize = true, ForeColor = Color.Black };
            _numSms = new NumericUpDown
            {
                Location = new Point(340, 58),
                Width = 130,
                Maximum = 1000,
                Value = 20
            };

            // Кнопка розрахунку
            _btnCalculate = new Button
            {
                Text = "Підібрати найвигідніший тариф",
                Location = new Point(20, 105),
                Size = new Size(450, 42),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnCalculate.FlatAppearance.BorderSize = 0;
            _btnCalculate.Click += BtnCalculate_Click;

            inputGroup.Controls.AddRange(new Control[] {
                lblGb, _numGb,
                lblMin, _numMinutes,
                lblSms, _numSms,
                _btnCalculate
            });

            // Картка результату
            _resultPanel = new Panel
            {
                Location = new Point(25, 270),
                Size = new Size(495, 240),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _lblResultTitle = new Label
            {
                Text = "Введіть параметри та натисніть кнопку розрахунку",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, 20),
                AutoSize = true
            };

            _lblResultPrice = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Location = new Point(20, 55),
                AutoSize = true
            };

            _lblResultDetails = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(20, 95),
                Size = new Size(450, 130)
            };

            _resultPanel.Controls.AddRange(new Control[] { _lblResultTitle, _lblResultPrice, _lblResultDetails });

            Controls.AddRange(new Control[] { lblHeader, lblSub, inputGroup, _resultPanel });
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            int reqGb = (int)_numGb.Value;
            int reqMinutes = (int)_numMinutes.Value;
            int reqSms = (int)_numSms.Value;

            Tariff? bestTariff = null;

            // --- Логіка відбору за допомогою if / else ---
            // 1. Перевірка тарифу "Базовий Економ" (120 грн)
            if (reqGb <= 5 && reqMinutes <= 150 && reqSms <= 50)
            {
                bestTariff = _tariffs[0];
            }
            // 2. Перевірка тарифу "Тільки Дзвінки" (150 грн)
            else if (reqGb <= 1 && reqMinutes <= 800 && reqSms <= 100)
            {
                bestTariff = _tariffs[1];
            }
            // 3. Перевірка тарифу "Комфортний Оптимум" (220 грн)
            else if (reqGb <= 25 && reqMinutes <= 500 && reqSms <= 150)
            {
                bestTariff = _tariffs[2];
            }
            // 4. Перевірка тарифу "Безліміт Макс Pro" (350 грн)
            else if (reqGb <= 100 && reqMinutes <= 1500 && reqSms <= 300)
            {
                bestTariff = _tariffs[3];
            }
            else
            {
                // Потреби перевищують стандартні рамки
                bestTariff = null;
            }

            // Виведення результатів
            if (bestTariff != null)
            {
                _lblResultTitle.Text = $"Рекомендований тариф: «{bestTariff.Name}»";
                _lblResultTitle.ForeColor = Color.FromArgb(37, 99, 235);
                _lblResultPrice.Text = $"{bestTariff.Price:N0} грн / місяць";
                _lblResultDetails.Text =
                    $"• Включено інтернету: {bestTariff.InternetGb} ГБ\n" +
                    $"• Включено дзвінків: {bestTariff.Minutes} хв\n" +
                    $"• Включено SMS: {bestTariff.Sms} шт.\n\n" +
                    $"Опис: {bestTariff.Description}";
            }
            else
            {
                _lblResultTitle.Text = "Індивідуальний або Корпоративний тариф";
                _lblResultTitle.ForeColor = Color.FromArgb(225, 29, 72);
                _lblResultPrice.Text = "Ціна за запитом";
                _lblResultDetails.Text =
                    "Ваші потреби перевищують ліміти стандартних пакетів.\n" +
                    "Рекомендуємо звернутися до контакт-центру для підключення необмеженого корпоративного тарифу.";
            }
        }
    }
}