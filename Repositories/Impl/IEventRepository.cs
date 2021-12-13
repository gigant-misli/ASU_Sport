﻿using ASUSport.Models;
using ASUSport.DTO;
using System.Collections.Generic;

namespace ASUSport.Repositories.Impl
{
    /// <summary>
    /// Репозиторий событий
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Регистрация пользователя на событие
        /// </summary>
        /// <param name="data">Форма для ввода праметров события</param>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Ответ</returns>
        public Response SignUpForAnEvent(int eventId, string login);

        /// <summary>
        /// Создание нового события
        /// </summary>
        /// <param name="data">Форма для ввода праметров события</param>
        /// <returns>Ответ</returns>
        public Response AddEvent(EventDTO data);

        /// <summary>
        /// Поиск тренера по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns></returns>
        public User GetTrainer(int id);

        /// <summary>
        /// Получить список событий по параметрам
        /// </summary>
        /// <param name="section">Идентификатор секции</param>
        /// <param name="trainer">Идентификатор тренера</param>
        /// <param name="date">Дата</param>
        /// <param name="time">Время</param>
        /// <returns>Список событий</returns>
        public List<EventModelDTO> GetEvents(int? section, int? trainer, string date, string time);

        /// <summary>
        /// Получить идентификатор события по параметру
        /// </summary>
        /// <param name="section">Идентификатор секции</param>
        /// <param name="trainer">Идентификатор тренера</param>
        /// <param name="date">Дата</param>
        /// <param name="time">Время</param>
        /// <returns>Идентификатор события</returns>
        public int GetEvent(int? section, int? trainer, string date, string time);

    }
}
