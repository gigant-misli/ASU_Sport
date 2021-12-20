﻿using System;
using System.Collections.Generic;
using System.Linq;
using ASUSport.Models;
using ASUSport.Repositories.Impl;
using ASUSport.DTO;

namespace ASUSport.Repositories
{
    public class EventRepository : IEventRepository
    {
        private ApplicationContext db;

        public EventRepository(ApplicationContext context)
        {
            db = context;
        }

        /// <inheritdoc/>
        public Response SignUpForAnEvent(int eventId, string login)
        {
            if (login == null)
            {
                return new Response()
                {
                    Status = false,
                    Type = "not_authorized",
                    Message = "Пользователь не авторизован"
                };
            }
            
            var selectedEvent = db.Events.FirstOrDefault(e => e.Id == eventId);

            if (selectedEvent == null)
            {
                return new Response()
                {
                    Status = false,
                    Type = "event_not_found",
                    Message = "События не существует"
                };
            }

            var user = db.Users.First(u => u.Login == login);

            if (user.Events.FirstOrDefault(e => e.Id == eventId) != null)
            {
                return new Response()
                {
                    Status = false,
                    Type = "already_signed_up",
                    Message = "Пользователь уже записан на это событие"
                };
            }

            if (selectedEvent.Section.SportObject.Capacity - selectedEvent.Clients.Count == 0)
            {
                return new Response()
                {
                    Status = false,
                    Type = "no_free_spaces",
                    Message = "Свободных мест нет"
                };
            }

            selectedEvent.Clients.Add(user);
            db.SaveChanges();

            return new Response()
            {
                Status = true,
                Type = "success",
                Message = "OK"
            };
        }

        /// <inheritdoc/>
        public Response AddEvent(EventDTO data)
        {
            var section = db.Sections.FirstOrDefault(s => s.Id == data.Section);

            if (section == null)
            {
                return new Response()
                {
                    Status = false,
                    Type = "section_not_found",
                    Message = "Секция не найдена"
                };
            }

            User selectedTrainer = null;

            if (data.Trainer != null)
            {
                selectedTrainer = GetTrainer((int)data.Trainer);

                if (selectedTrainer == null)
                {
                    return new Response()
                    {
                        Status = false,
                        Type = "trainer_not_found",
                        Message = "Тренер не найден"
                    };
                }
            }

            var newEvent = new Event()
            {
                Section = section,
                Trainer = selectedTrainer,
                Time = DateTime.Parse(data.Date + " " + data.Time)
            };

            db.Events.Add(newEvent);
            db.SaveChanges();

            return new Response()
            {
                Status = true,
                Type = "success",
                Message = "OK"
            };
        }

        /// <inheritdoc/>
        public User GetTrainer(int id)
        {
            var trainer = db.UserData.FirstOrDefault(u => u.User.Id == id);

            if (trainer == null)
                return null;

            return trainer.User;
        }

        /// <inheritdoc/>
        public List<EventModelDTO> GetEvents(int? section, int? trainer, string date, string time)
        {
            var result = new List<EventModelDTO>();

            List<Event> events = db.Events.Where(e => e.Time > DateTime.Now).Select(e => e).ToList();

            if (section != null)
            {
                events = events.Where(e => e.Section.Id == section).ToList();
            }

            if (date != null)
            {
                events = events.Where(e => e.Time.Date == DateTime.Parse(date)).ToList();
            }

            if (time != null)
            {
                events = events.Where(e => e.Time.ToString("HH:mm") == time).ToList();
            }

            if (trainer != null)
            {
                var trainerUser = GetTrainer((int)trainer);

                events = events.Where(e => e.Trainer == trainerUser).ToList();
            }

            foreach (Event ev in events.ToList())
            {
                string trainerName = string.Empty;
                
                if (ev.Trainer != null)
                {
                    var trainerUser = db.UserData.First(u => u.User == ev.Trainer);

                    trainerName = trainerUser.FirstName + " " + trainerUser.MiddleName + " " + trainerUser.LastName;
                }

                var capacity = ev.Section.SportObject.Capacity;

                var model = new EventModelDTO()
                {
                    Id = ev.Id,
                    SectionName = ev.Section.Name,
                    TrainerName = trainerName,
                    Date = ev.Time.ToString("yyyy-MM-dd"),
                    Time = ev.Time.ToString("HH:mm"),
                    Duration = ev.Section.Duration,
                    FreeSpaces = capacity - ev.Clients.Count,
                    Capacity = capacity
                };

                result.Add(model);
            }

            return result;
        }

        /// <inheritdoc/>
        public EventsForSportobjectDTO GetEventByDateSportObject(int id, string date)
        {
            var events = db.Events.Where(e => e.Section.SportObject.Id == id && e.Time.Date == DateTime.Parse(date)).ToList();

            var eventsList = new List<EventModelDTO>();

            var selectedObject = db.SportObjects.First(s => s.Id == id);

            int capacity = selectedObject.Capacity;
            string name = selectedObject.Name;

            foreach (var e in events)
            {
                string trainerName = string.Empty;

                if (e.Trainer != null)
                {
                    var trainer = db.UserData.First(u => u.User.Id == e.Trainer.Id);

                    trainerName = trainer.FirstName + " " + trainer.MiddleName + " " + trainer.LastName;
                }

                var model = new EventModelDTO()
                {
                    Id = e.Id,
                    SectionName = e.Section.Name,
                    Time = e.Time.ToString("HH:mm"),
                    Duration = e.Section.Duration,
                    FreeSpaces = capacity - e.Clients.Count,
                    TrainerName = trainerName
                };

                eventsList.Add(model);
            }

            var dto = new EventsForSportobjectDTO()
            {
                ObjectName = name,
                Capacity = capacity,
                Date = date,
                Events = eventsList
            };

            return dto;
        }

        /// <inheritdoc/>
        public int GetEvent(int? section, int? trainer, string date, string time)
        {
            User trainerUser = null;

            if (trainer != null)
            {
                trainerUser = GetTrainer((int)trainer);

                if (trainer == null)
                {
                    return 0;
                }
            }

            var selectedEvent = db.Events.FirstOrDefault(
                e => e.Trainer == trainerUser && e.Section.Id == section && e.Time == DateTime.Parse(date + " " + time));

            if (selectedEvent == null)
            {
                return 0;
            }

            return selectedEvent.Id;
        }

        /// <inheritdoc/>
        public Response SignUpForUnathorized(SignUpForUnathorizedDTO data)
        {
            var selectedEvent = db.Events.First(o => o.Id == data.EventId);

            var newUser = new User()
            {
                Login = data.Login,
                RoleId = db.Roles.First(r => r.Name == "client").Id
            };

            db.Users.Add(newUser);

            var userData = new UserData()
            {
                FirstName = data.Name,
                User = newUser
            };

            db.UserData.Add(userData);

            selectedEvent.Clients.Add(newUser);

            db.SaveChanges();

            return new Response()
            {
                Status = true,
                Type = "success",
                Message = "OK"
            };
        }
    }
}
