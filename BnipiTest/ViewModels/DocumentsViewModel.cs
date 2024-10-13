namespace BnipiTest.ViewModels
{
    public class DocumentsViewModel
    {
        //1. Шифр проекта
        //2. Код объекта проектирования(полный код объекта формируется как  Полный код родительского объекта.Собственный код)
        //3. Марка
        //4. Номер(нумеруются комплекты с нуля). Марка + Номер
        //5. Полный шифр комплекта(формируется как Шифр проекта-Полный код объекта-МаркаНомер)
        public string ProjectCipher { get; set; }
        public string DesignObjectCode { get; set; }
        public string Mark { get; set; }
        public string Number { get; set; }
        public string FullDocumentCipher { get; set; }
    }
}
