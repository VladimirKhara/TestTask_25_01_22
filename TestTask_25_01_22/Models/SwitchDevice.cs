using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace TestTask_25_01_22.Models
{
    public partial class SwitchDevice
    {

        public SwitchDevice()
        { }

        public SwitchDevice(string v1, string v2, string v3, string v4, string v5, string v6, DateTime dateTime1, DateTime dateTime2)
        {
            ModelName = v1;
            Ipv4 = v2;
            Mac = v3;
            MainVlan = v4;
            SerialNumber = v5;
            InventoryNumber = v6;
            PurchaseDate = dateTime1;
            InstallationDate = dateTime2;
        }

        [Display(Name = "Название модели")]
        [Required(ErrorMessage = "Введите название модели")]
        public string ModelName { get; set; }

        [Display(Name = "IP-адрес")]
        [Required(ErrorMessage = "Введите IP-адрес")]
        [RegularExpression(@"(\d{1,3}).(\d{1,3}).(\d{1,3}).(\d{1,3})", ErrorMessage = "Некорректный IP-адрес")]
        //[Remote(action: "IsNotExistsIP", controller: "SwitchDevices", ErrorMessage = "IP-адрес уже используется")]
        public string Ipv4 { get; set; }

        [Display(Name = "MAC-адрес")]
        [Required(ErrorMessage = "Введите MAC-адрес")]
        [RegularExpression(@"^[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}$", ErrorMessage = "Некорректный MAC-адрес")]
        //[Remote(action: "IsNotExistsMAC", controller: "SwitchDevices", ErrorMessage = "MAC-адрес занят")]
        public string Mac { get; set; }

        [Display(Name = "VLAN")]
        [Required(ErrorMessage = "Введите VLAN")]
        [RegularExpression(@"(\d{1,3}).(\d{1,3}).(\d{1,3}).(\d{1,3})", ErrorMessage = "Некорректный адрес VLAN")]
        //[Remote(action: "IsNotExistsVLAN", controller: "SwitchDevices", ErrorMessage = "VLAN-адрес уже используется")]
        public string MainVlan { get; set; }

        [Display(Name = "Cерийный номер")]
        [Required(ErrorMessage = "Введите серийный номер")]
        public string SerialNumber { get; set; }

        [Display(Name = "Инвентарный номер")]
        [Required(ErrorMessage = "Введите инвентарный номер")]
        //[Remote(action: "IsNotExistsInventoryNumber", controller: "SwitchDevices", ErrorMessage = "Инвентарный номер занят")]
        public string InventoryNumber { get; set; }

        [Display(Name = "Дата закупки")]
        [Required(ErrorMessage = "Введите дату закупки")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Дата установки")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? InstallationDate { get; set; }

        [Display(Name = "Номер этажа")]
        [Range(0, 10, ErrorMessage = "Такого этажа нет")]
        public byte? Stage { get; set; }

        [Display(Name = "Комментарий")]
        [StringLength(500, ErrorMessage = "Длина строки должна быть до 500 символов")]
        public string Comment { get; set; }
    }
}
