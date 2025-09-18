using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class MainForm : Form
        {
            private Player player;
            private Enemy currentEnemy;

            // UI елементи
            private Label lblPlayerStats;
            private Label lblWeapon;
            private Label lblArmor;
            private Label lblEnemy;
            private Button btnAttack;
            private Button btnNewEnemy;
            private ListBox lstLog;

            public MainForm()
            {
                Text = "Simple RPG - Етап 1";
                Size = new Size(700, 500);
                StartPosition = FormStartPosition.CenterScreen;

                InitGame();
                InitializeComponents();
                RefreshUI();
            }

            private void InitGame()
            {
                player = new Player(str: 6, end: 6, agi: 5, intel: 4);
                currentEnemy = GenerateEnemy(1);
            }

            private Enemy GenerateEnemy(int level)
            {
                if (level == 1)
                    return new Enemy("Дикий вовк", 1, 18, 4, 1, 30, 10);
                if (level == 2)
                    return new Enemy("Бандит", 2, 28, 6, 2, 60, 25);
                return new Enemy("Гігант", level, 40 + level * 10, 8 + level * 2, 3 + level, 100 * level, 50 * level);
            }

            private void InitializeComponents()
            {
                lblPlayerStats = new Label() { Location = new Point(10, 10), Size = new Size(320, 140) };
                lblWeapon = new Label() { Location = new Point(10, 160), Size = new Size(320, 20) };
                lblArmor = new Label() { Location = new Point(10, 185), Size = new Size(320, 20) };

                lblEnemy = new Label() { Location = new Point(350, 10), Size = new Size(320, 120) };

                btnAttack = new Button() { Location = new Point(350, 140), Size = new Size(120, 30), Text = "Атакувати" };
                btnAttack.Click += BtnAttack_Click;

                btnNewEnemy = new Button() { Location = new Point(480, 140), Size = new Size(120, 30), Text = "Новий ворог" };
                btnNewEnemy.Click += BtnNewEnemy_Click;

                lstLog = new ListBox() { Location = new Point(10, 220), Size = new Size(660, 220) };

                Controls.Add(lblPlayerStats);
                Controls.Add(lblWeapon);
                Controls.Add(lblArmor);
                Controls.Add(lblEnemy);
                Controls.Add(btnAttack);
                Controls.Add(btnNewEnemy);
                Controls.Add(lstLog);
            }

            private void BtnAttack_Click(object sender, EventArgs e)
            {
                if (currentEnemy == null) return;
                if (currentEnemy.IsDead)
                {
                    Log("Цей ворог вже повалений.");
                    return;
                }

                int playerAtk = player.CalculateAttack();
                int damageToEnemy = Math.Max(1, playerAtk - currentEnemy.Defense);
                currentEnemy.TakeDamage(damageToEnemy);
                Log($"Ви завдали {damageToEnemy} пошкодження {currentEnemy.Name} (HP {currentEnemy.Health}/{currentEnemy.MaxHealth})");

                if (currentEnemy.IsDead)
                {
                    Log($"Ви перемогли {currentEnemy.Name}! Отримано {currentEnemy.ExperienceReward} досвіду та {currentEnemy.GoldReward} золота.");
                    player.GainExperience(currentEnemy.ExperienceReward);
                    player.GainGold(currentEnemy.GoldReward);
                    RefreshUI();
                    return;
                }

                int enemyAtk = currentEnemy.Attack;
                int damageToPlayer = Math.Max(1, enemyAtk - player.CalculateDefense());
                player.TakeDamage(damageToPlayer);
                Log($"{currentEnemy.Name} завдав вам {damageToPlayer} пошкодження (HP {player.Health}/{player.MaxHealth})");

                if (player.Health <= 0)
                {
                    Log("Ви загинули. Гру можна перезапустити (перезапустіть програму або додайте кнопку респавну).");
                }

                RefreshUI();
            }

            private void BtnNewEnemy_Click(object sender, EventArgs e)
            {
                int nextLevel = currentEnemy?.Level + 1 ?? 1;
                currentEnemy = GenerateEnemy(nextLevel);
                Log($"Зʼявився ворог: {currentEnemy.Name} (рівень {currentEnemy.Level})");
                RefreshUI();
            }

            private void RefreshUI()
            {
                lblPlayerStats.Text = $"Рівень: {player.Level}  Досвід: {player.Experience}/{100 * player.Level}\n" +
                                      $"Здоров'я: {player.Health}/{player.MaxHealth}  Мана: {player.Mana}/{player.MaxMana}\n" +
                                      $"Сила: {player.Strength}  Витривалість: {player.Endurance}  Спритність: {player.Agility}  Інтелект: {player.Intelligence}\n" +
                                      $"Золото: {player.Gold}";

                lblWeapon.Text = "Зброя: " + (player.EquippedWeapon?.ToString() ?? "Немає");
                lblArmor.Text = "Броня: " + (player.EquippedArmor?.ToString() ?? "Немає");

                if (currentEnemy != null)
                {
                    lblEnemy.Text = $"Ворог: {currentEnemy.Name} (Рівень {currentEnemy.Level})\nHP: {currentEnemy.Health}/{currentEnemy.MaxHealth}\nATK: {currentEnemy.Attack}  DEF: {currentEnemy.Defense}";
                }
                else
                {
                    lblEnemy.Text = "Немає ворога";
                }
            }

            private void Log(string text)
            {
                lstLog.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {text}");
            }

            private void InitializeComponent()
            {
                this.SuspendLayout();
                // 
                // MainForm
                // 
                this.ClientSize = new System.Drawing.Size(284, 261);
                this.Name = "MainForm";
                this.Load += new System.EventHandler(this.MainForm_Load);
                this.ResumeLayout(false);

            }

            private void MainForm_Load(object sender, EventArgs e)
            {

            }
        }

        // Клас гравця
        public class Player
        {
            public int Strength { get; set; }
            public int Endurance { get; set; }
            public int Agility { get; set; }
            public int Intelligence { get; set; }

            public int MaxHealth { get; private set; }
            public int Health { get; private set; }
            public int MaxMana { get; private set; }
            public int Mana { get; private set; }
            public int Gold { get; private set; }
            public int Level { get; private set; }
            public int Experience { get; private set; }

            public Weapon EquippedWeapon { get; private set; }
            public Armor EquippedArmor { get; private set; }

            public Player(int str = 5, int end = 5, int agi = 5, int intel = 5)
            {
                Strength = str;
                Endurance = end;
                Agility = agi;
                Intelligence = intel;

                Level = 1;
                Experience = 0;
                Gold = 50;

                RecalculateDerived();
                HealFull();
                RestoreManaFull();

                EquippedWeapon = new Weapon("Дерев'яний меч", 0, 2);
                EquippedArmor = new Armor("Шкіряна броня", 0, 1);
            }

            public void RecalculateDerived()
            {
                MaxHealth = 20 + Endurance * 5 + Strength * 2;
                MaxMana = 10 + Intelligence * 3;
            }

            public void HealFull() => Health = MaxHealth;
            public void RestoreManaFull() => Mana = MaxMana;

            public int CalculateAttack()
            {
                int baseAttack = Strength + (EquippedWeapon?.AttackPower ?? 0);
                int bonus = Agility / 3;
                return baseAttack + bonus;
            }

            public int CalculateDefense()
            {
                int baseDef = Endurance + (EquippedArmor?.Defense ?? 0);
                int bonus = Agility / 4;
                return baseDef + bonus;
            }

            public void TakeDamage(int dmg)
            {
                Health -= dmg;
                if (Health < 0) Health = 0;
            }

            public void SpendMana(int amount)
            {
                Mana -= amount;
                if (Mana < 0) Mana = 0;
            }

            public void GainGold(int amount) => Gold += amount;
            public bool SpendGold(int amount)
            {
                if (Gold < amount) return false;
                Gold -= amount;
                return true;
            }

            public void GainExperience(int exp)
            {
                Experience += exp;
                while (Experience >= 100 * Level)
                {
                    Experience -= 100 * Level;
                    LevelUp();
                }
            }

            private void LevelUp()
            {
                Level++;
                Strength += 1;
                Endurance += 1;
                Agility += 1;
                Intelligence += 1;
                RecalculateDerived();
                HealFull();
                RestoreManaFull();
            }

            public void EquipWeapon(Weapon w) => EquippedWeapon = w;
            public void EquipArmor(Armor a) => EquippedArmor = a;
        }

        public class Enemy
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int MaxHealth { get; private set; }
            public int Health { get; private set; }
            public int Attack { get; private set; }
            public int Defense { get; private set; }
            public int ExperienceReward { get; private set; }
            public int GoldReward { get; private set; }

            public Enemy(string name, int level, int health, int attack, int defense, int expReward, int goldReward)
            {
                Name = name;
                Level = level;
                MaxHealth = health;
                Health = health;
                Attack = attack;
                Defense = defense;
                ExperienceReward = expReward;
                GoldReward = goldReward;
            }

            public void TakeDamage(int dmg)
            {
                Health -= dmg;
                if (Health < 0) Health = 0;
            }

            public bool IsDead => Health <= 0;
        }

        public class Weapon
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int AttackPower { get; set; }

            public Weapon(string name, int price, int attackPower)
            {
                Name = name;
                Price = price;
                AttackPower = attackPower;
            }

            public override string ToString() => $"{Name} (+{AttackPower} ATK)";
        }

        public class Armor
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int Defense { get; set; }

            public Armor(string name, int price, int defense)
            {
                Name = name;
                Price = price;
                Defense = defense;
            }

            public override string ToString() => $"{Name} (+{Defense} DEF)";
        }

        
    }
}

