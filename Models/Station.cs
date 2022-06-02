namespace Modules_Replacer
{
    public class Station
    {
        /// <summary>
        /// Макрос
        /// </summary>
        public string Macros { get; set; }
        /// <summary>
        /// Название модуля
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Цвет шрифта
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Модуль
        /// </summary>
        /// <param name="macros">Макрос</param>
        /// <param name="name">Имя</param>
        public Station(string macros, string name)
        {
            Macros = macros;
            Name = name;
        }
        /// <summary>
        /// Установка цвета шрифта.
        /// </summary>
        /// <param name="color">Цвет в int</param>
        public void SetForeground(int color)
        {
            Color = $"#{color.ToString("X")}";
        }
    }
}
