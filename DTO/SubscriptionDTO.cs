﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASUSport.DTO
{
    /// <summary>
    /// Модель для представления информации о абонементе
    /// </summary>
    public class SubscriptionDTO
    {
        /// <summary>
        /// название спротивного объекта
        /// </summary>
        public string SportObjectName { get; set; }

        /// <summary>
        /// Тип абонемента
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Название абонемента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Кол-во посещений
        /// </summary>
        public int NumOfVisits { get; set; }

        /// <summary>
        /// Стоимость
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Время открытия
        /// </summary>
        public string StartingTime { get; set; }

        /// <summary>
        /// Время закрытия
        /// </summary>
        public string ClosingTime { get; set; }
    }
}
