using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Modules_Replacer
{
    public class ModuleVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля
        /// <summary>
        /// Видимость кнопок со стрелками
        /// </summary>
        private Visibility vis;
        /// <summary>
        /// Коллекция модулей
        /// </summary>
        private ViewModel mainvm;
        /// <summary>
        /// Выбор модуля
        /// </summary>
        private bool selected;
        /// <summary>
        /// Вставить ниже
        /// </summary>
        private RelayCommand pastedown;
        /// <summary>
        /// Вставить выше
        /// </summary>
        private RelayCommand pasteup;
        /// <summary>
        /// Название модуля
        /// </summary>
        private string name;
        /// <summary>
        /// Цвет текста
        /// </summary>
        private string fg;
        /// <summary>
        /// Цвет фона
        /// </summary>
        private string bg;
        #endregion

        #region Команды
        /// <summary>
        /// Вставить ниже
        /// </summary>
        public RelayCommand PasteDOWN => pastedown ?? (pastedown = new RelayCommand(obj => PasteDown()));
        /// <summary>
        /// Вставить выше
        /// </summary>
        public RelayCommand PasteUP => pasteup ?? (pasteup = new RelayCommand(obj => PasteUp()));
        #endregion

        #region Свойства
        /// <summary>
        /// Видимость кнопок со стрелками
        /// </summary>
        public Visibility Vis
        {
            get => vis;
            set
            {
                vis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vis)));
            }
        }
        /// <summary>
        /// Коллекция модулей
        /// </summary>
        public ViewModel MainVM
        {
            get => mainvm;
            set
            {
                mainvm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainVM)));
            }
        }
        /// <summary>
        /// Выбор модуля
        /// </summary>
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                if (MainVM != null)
                {
                    if (value)
                    {
                        if (Vis == Visibility.Visible)
                        {
                            MainVM.Coll++;
                        }
                        Vis = Visibility.Hidden;
                    }
                    else
                    {
                        if (Vis == Visibility.Hidden)
                        {
                            MainVM.Coll--;
                        }
                        Vis = Visibility.Visible;
                    }
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
        /// <summary>
        /// Название модуля
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        /// <summary>
        /// Цвет текста
        /// </summary>
        public string FG
        {
            get => fg;
            set
            {
                fg = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FG)));
            }
        }
        /// <summary>
        /// Цвет фона
        /// </summary>
        public string BG
        {
            get => bg;
            set
            {
                bg = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BG)));
            }
        }
        #endregion

        #region Авто-Свойства
        /// <summary>
        /// Идентификатор модуля.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название макроса.
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// Json текст.
        /// </summary>
        public string Text { get; set; }
        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="name">Название модуля</param>
        /// <param name="fg">Цвет текста</param>
        /// <param name="bg">Цвет фона</param>
        public ModuleVM(int id, string name, string truename, string text, string fg, string bg, ViewModel mainvm)
        {
            Id = id;
            Name = name;
            TrueName = truename;
            FG = fg;
            BG = bg;
            Selected = false;
            MainVM = mainvm;
            Text = text;
            Vis = Visibility.Visible;
        }

        /// <summary>
        /// Вставить выше
        /// </summary>
        private void PasteUp()
        {
            MainVM.Paste(Id, true);
        }
        /// <summary>
        /// Вставить ниже
        /// </summary>
        private void PasteDown()
        {
            MainVM.Paste(Id, false);
        }
    }
}

