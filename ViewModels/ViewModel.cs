using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Modules_Replacer
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля
        /// <summary>
        /// Коллекция вью моделей
        /// </summary>
        private List<ModuleVM> vmcollection;
        /// <summary>
        /// Выбранный модуль
        /// </summary>
        private ModuleUI selected;
        /// <summary>
        /// Список модулей
        /// </summary>
        private ObservableCollection<ModuleUI> modlist;
        /// <summary>
        /// Тип модуля
        /// </summary>
        private int types;
        /// <summary>
        /// Последовательное наращивание производства
        /// </summary>
        private RelayCommand sortstepbystep;
        /// <summary>
        /// Выделено модулей
        /// </summary>
        private int coll;
        /// <summary>
        /// Выкл все
        /// </summary>
        private RelayCommand unselectstructs;
        /// <summary>
        /// Вкл все
        /// </summary>
        private RelayCommand selectstructs;
        /// <summary>
        /// Удалить отмеченное
        /// </summary>
        private string element;
        /// <summary>
        /// Удалить отмеченное
        /// </summary>
        private RelayCommand removeselected;
        /// <summary>
        /// Сохранить станцию как
        /// </summary>
        private RelayCommand saveasfile;
        /// <summary>
        /// Сохранить станцию
        /// </summary>
        private RelayCommand savefile;
        /// <summary>
        /// Загрузить станцию
        /// </summary>
        private RelayCommand loadfile;
        #endregion

        #region Свойства
        /// <summary>
        /// Коллекция вью моделей
        /// </summary>
        public List<ModuleVM> VMCollection
        {
            get => vmcollection;
            set
            {
                vmcollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VMCollection)));
            }
        }
        /// <summary>
        /// Выбранный модуль
        /// </summary>
        public ModuleUI Selected
        {
            get => selected;
            set
            {
                selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
        /// <summary>
        /// Список модулей
        /// </summary>
        public ObservableCollection<ModuleUI> ModList
        {
            get => modlist;
            set
            {
                modlist = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModList)));
            }
        }
        /// <summary>
        /// Тип модуля
        /// </summary>
        public int Types
        {
            get => types;
            set
            {
                types = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }
        /// <summary>
        /// Выделено модулей
        /// </summary>
        public int Coll
        {
            get => coll;
            set
            {
                coll = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Coll)));
            }
        }
        /// <summary>
        /// Удалить отмеченное
        /// </summary>
        public string Element
        {
            get => element;
            set
            {
                element = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Element)));
            }
        }
        #endregion

        #region Команды
        /// <summary>
        /// Последовательное наращивание производства
        /// </summary>
        public RelayCommand SortStepByStep => sortstepbystep ?? (sortstepbystep = new RelayCommand(obj => StepByStep()));
        /// <summary>
        /// Выкл все
        /// </summary>
        public RelayCommand UnSelectStructs => unselectstructs ?? (unselectstructs = new RelayCommand(obj => UnSelectAllConnectors()));
        /// <summary>
        /// Вкл все
        /// </summary>
        public RelayCommand SelectStructs => selectstructs ?? (selectstructs = new RelayCommand(obj => SelectAllConnectors()));
        /// <summary>
        /// Удалить отмеченное
        /// </summary>
        public RelayCommand RemoveSelected => removeselected ?? (removeselected = new RelayCommand(obj => RemoveModules()));
        /// <summary>
        /// Сохранить станцию как
        /// </summary>
        public RelayCommand SaveAsFile => saveasfile ?? (saveasfile = new RelayCommand(obj => SaveStationAs()));
        /// <summary>
        /// Сохранить станцию
        /// </summary>
        public RelayCommand SaveFile => savefile ?? (savefile = new RelayCommand(obj => SaveStation()));
        /// <summary>
        /// Загрузить станцию
        /// </summary>
        public RelayCommand LoadFile => loadfile ?? (loadfile = new RelayCommand(obj => LoadStation()));
        #endregion

        #region Авто-Свойства
        public static string CurStation { get; set; }
        public static string CurStationName { get; set; }
        static string Station { get; set; }
        static string[] Modules { get; set; }
        /// <summary>
        /// Временный рабочий список модулей.
        /// </summary>
        public List<ModuleUI> TempList { get; set; }
        private List<List<ModuleVM>> Factories { get; set; }
        private List<List<ModuleUI>> FactoriesUI { get; set; }
        public List<ModuleUI> SelectedList { get; set; }
        public List<ModuleUI> UnSelectedList { get; set; }
        public List<ModuleVM> VMSelected { get; set; }
        public List<ModuleVM> VMUnSelected { get; set; }
        public bool Up { get; set; }
        #endregion

        public ViewModel()
        {
            Types = 6;
            ModList = new ObservableCollection<ModuleUI>();
            TempList = new List<ModuleUI>();
            VMCollection = new List<ModuleVM>();
        }

        /// <summary>
        /// Загрузить станцию
        /// </summary>
        private void LoadStation()
        {
            OpenFileDialog GetFile = new OpenFileDialog
            {
                Title = "Выберите файл станции",
                Filter = "XML файл (*.xml)| *.xml; *.XML"
            };
            if (GetFile.ShowDialog() == true)
            {
                CurStation = GetFile.FileName;
                CurStationName = GetFile.SafeFileName;
                ModList.Clear();
                VMCollection.Clear();
                Station = File.ReadAllText(CurStation);
                Station = Station.Substring(0, Station.IndexOf("</plan>"));
                string[] patt = new string[1];
                patt[0] = "<entry index=\"";
                Modules = Station.Split(patt, StringSplitOptions.None);
                for (int i = 1; i < Modules.Length; i++)
                {
                    ModuleUI template = new ModuleUI();
                    string modulname;
                    string color;
                    try 
                    {
                        modulname = ModulesCollection.StationNames.Find(u => u.Macros == GetName(Modules[i])).Name;
                        color = ModulesCollection.StationNames.Find(u => u.Macros == GetName(Modules[i])).Color;
                    }
                    catch
                    {
                        modulname = GetName(Modules[i]);
                        color = "#FFFFFF";
                    }
                    string modultext = Modules[i].Substring(Modules[i].IndexOf("\""));
                    if (modultext.IndexOf("<predecessor") != -1) { modultext = RemovePredecessor(modultext); }
                    ModuleVM viewmodel = new ModuleVM(i, modulname, GetName(Modules[i]), modultext, color, "0", this);
                    VMCollection.Add(viewmodel);
                    template.DataContext = viewmodel;
                    ModList.Add(template);
                }
                Coll = 0;
            }
        }
        /// <summary>
        /// Сохранить станцию
        /// </summary>
        private void SaveStation()
        {
            string Output = $"{Modules[0].Substring(0, Modules[0].IndexOf("name=\"") + "name=\"".Length)}" +
                $"{CurStationName.Substring(0, CurStationName.LastIndexOf("."))}" +
                $"{Modules[0].Substring(Modules[0].IndexOf("\" description"))}";
            for (int i = 0; i < VMCollection.Count; i++)
            {
                Output += $"<entry index=\"{i + 1}{VMCollection[i].Text}";
            }
            Output += "</plan>\n</plans>\n";
            File.WriteAllText(CurStation, Output);
        }
        /// <summary>
        /// Сохранить станцию как
        /// </summary>
        private void SaveStationAs()
        {
            SaveFileDialog SFD = new SaveFileDialog()
            {
                Title = "Сохранить файл как",
                DefaultExt = ".xml",
                AddExtension = true
            };
            if (SFD.ShowDialog() == true)
            {
                CurStationName = SFD.SafeFileName;
                CurStation = SFD.FileName;
                SaveStation();
            }
        }
        /// <summary>
        /// Удалить отмеченное
        /// </summary>
        private void RemoveModules()
        {
            List<ModuleVM> Temp = new List<ModuleVM>();
            TempList.Clear();
            TempList.AddRange(ModList);
            ModList.Clear();
            for (int i = 0; i < TempList.Count; i++)
            {
                if (!VMCollection[i].Selected)
                {
                    Temp.Add(VMCollection[i]);
                    ModList.Add(TempList[i]);
                }
            }
            VMCollection = new List<ModuleVM>(Temp);
            Coll = 0;
        }
        /// <summary>
        /// Вкл все
        /// </summary>
        private void SelectAllConnectors()
        {
            for (int i = 0; i < ModList.Count; i++)
            {
                if (Types == 0 && VMCollection[i].TrueName.StartsWith("buildmodule_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 1 && VMCollection[i].TrueName.StartsWith("storage_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 2 && VMCollection[i].TrueName.StartsWith("hab_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 3 && VMCollection[i].TrueName.StartsWith("dockarea_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 4 && VMCollection[i].TrueName.StartsWith("pier_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 5 && VMCollection[i].TrueName.StartsWith("defence_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 6 && VMCollection[i].TrueName.StartsWith("struct_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 7 && VMCollection[i].TrueName.StartsWith("prod_"))
                {
                    VMCollection[i].Selected = true;
                }
                else if (Types == 8 && VMCollection[i].Name.IndexOf(Element) != -1)
                {
                    VMCollection[i].Selected = true;
                }
            }
        }
        /// <summary>
        /// Выкл все
        /// </summary>
        private void UnSelectAllConnectors()
        {
            for (int i = 0; i < ModList.Count; i++)
            {
                if (Types == 0 && VMCollection[i].TrueName.StartsWith("buildmodule_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 1 && VMCollection[i].TrueName.StartsWith("storage_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 2 && VMCollection[i].TrueName.StartsWith("hab_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 3 && VMCollection[i].TrueName.StartsWith("dockarea_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 4 && VMCollection[i].TrueName.StartsWith("pier_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 5 && VMCollection[i].TrueName.StartsWith("defence_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 6 && VMCollection[i].TrueName.StartsWith("struct_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 7 && VMCollection[i].TrueName.StartsWith("prod_"))
                {
                    VMCollection[i].Selected = false;
                }
                else if (Types == 8 && VMCollection[i].Name.IndexOf(Element) != -1)
                {
                    VMCollection[i].Selected = false;
                }
            }
        }
        /// <summary>
        /// Последовательное наращивание производства
        /// </summary>
        private void StepByStep()
        {
            Factories = new List<List<ModuleVM>>();
            FactoriesUI = new List<List<ModuleUI>>();
            List<ModuleVM> TempVM = new List<ModuleVM>();
            TempList.Clear();
            for (int i = 0; i < ModList.Count; i++)
            {
                if (VMCollection[i].TrueName.Contains("prod_") || VMCollection[i].TrueName.Contains("hab_") || VMCollection[i].TrueName.Contains("storage_"))
                {
                    if (Factories == null)
                    {
                        Factories.Add(new List<ModuleVM>());
                        FactoriesUI.Add(new List<ModuleUI>());
                        Factories[0].Add(VMCollection[i]);
                        FactoriesUI[0].Add(ModList[i]);
                    }
                    else
                    {
                        bool find = false;
                        for (int j = 0; j < Factories.Count; j++)
                        {
                            if (Factories[j][0].TrueName == VMCollection[i].TrueName)
                            {
                                Factories[j].Add(VMCollection[i]);
                                FactoriesUI[j].Add(ModList[i]);
                                find = true;
                            }
                        }
                        if (!find)
                        {
                            Factories.Add(new List<ModuleVM>());
                            Factories[Factories.Count - 1].Add(VMCollection[i]);
                            FactoriesUI.Add(new List<ModuleUI>());
                            FactoriesUI[Factories.Count - 1].Add(ModList[i]);
                        }
                    }
                }
                else
                {
                    TempList.Add(ModList[i]);
                    TempVM.Add(VMCollection[i]);
                }
            }
            if (Factories.Count > 0)
            {
                int prod = Factories[Factories.Count - 1].Count;
                while (Factories[Factories.Count - 1].Count > 0)
                {
                    prod = Factories[Factories.Count - 1].Count;
                    for (int j = 0; j < Factories.Count; j++)
                    {
                        if (Factories[j].Count > 0)
                        {
                            int full = Factories[j].Count / prod;
                            if (full > 0)
                            {
                                if (Factories[j].Count - full * prod > 0)
                                {
                                    full++;
                                }
                            }
                            else
                            {
                                full = 1;
                            }
                            for (int k = 0; k < full; k++)
                            {
                                TempList.Add(FactoriesUI[j][0]);
                                TempVM.Add(Factories[j][0]);
                                Factories[j].RemoveAt(0);
                                FactoriesUI[j].RemoveAt(0);
                            }
                        }
                    }
                }
                ModList.Clear();
                for (int i = 0; i < TempList.Count; i++)
                {
                    ModList.Add(TempList[i]);
                }
                VMCollection = new List<ModuleVM>(TempVM);
            }
        }
        /// <summary>
        /// Получить имя модуля.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string GetName(string text)
        {
            string modulename = text.Substring(text.IndexOf("macro=\"") + "macro=\"".Length);
            modulename = modulename.Substring(0, modulename.IndexOf("\""));
            return modulename;
        }
        /// <summary>
        /// Удалить блок Predecessor.
        /// </summary>
        private string RemovePredecessor(string modultext)
        {
            string removetext = modultext;
            removetext = removetext.Substring(removetext.IndexOf("<predecessor"));
            removetext = removetext.Substring(0, removetext.IndexOf("<offset"));
            return modultext.Replace(removetext, "");
        }
        /// <summary>
        /// Вставить выбранные модули.
        /// </summary>
        /// <param name="Id">Id Модуля относительно которого делается вставка</param>
        /// <param name="up">Выше/Ниже</param>
        public void Paste(int Id, bool up)
        {
            GetSelected();
            TempList.Clear();
            ModList.Clear();
            VMCollection.Clear();
            for (int i = 0; i < UnSelectedList.Count; i++)
            {
                if (VMUnSelected[i].Id != Id)
                {
                    TempList.Add(UnSelectedList[i]);
                    VMCollection.Add(VMUnSelected[i]);
                }
                else
                {
                    if (up)
                    {
                        TempList.AddRange(SelectedList);
                        VMCollection.AddRange(VMSelected);
                        TempList.Add(UnSelectedList[i]);
                        VMCollection.Add(VMUnSelected[i]);
                    }
                    else
                    {
                        VMCollection.Add(VMUnSelected[i]);
                        TempList.Add(UnSelectedList[i]);
                        VMCollection.AddRange(VMSelected);
                        TempList.AddRange(SelectedList);
                    }
                }
            }
            ModList = new ObservableCollection<ModuleUI>(TempList);
        }
        /// <summary>
        /// Получить списки выбранных и не выбранных модулей.
        /// </summary>
        private void GetSelected()
        {
            VMSelected = new List<ModuleVM>();
            VMUnSelected = new List<ModuleVM>();
            SelectedList = new List<ModuleUI>();
            UnSelectedList = new List<ModuleUI>();
            for (int i = 0; i < ModList.Count; i++)
            {
                if (VMCollection[i].Selected)
                {
                    VMCollection[i].Selected = false;
                    VMSelected.Add(VMCollection[i]);
                    SelectedList.Add(ModList[i]);
                }
                else
                {
                    VMUnSelected.Add(VMCollection[i]);
                    UnSelectedList.Add(ModList[i]);
                }
            }
        }
    }
}