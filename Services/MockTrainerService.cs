using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services
{
    public class MockTrainerService : ITrainerService
    {
        private readonly List<Trainer> _trainers = new()
        {
            new Trainer
            {
                Id = 1,
                FirstName = "Анна",
                LastName = "Соколова",
                Direction = "yoga",
                Email = "anna@rockstar.ru",
                PasswordHash = "hashed_1",
                Experience = 5,
                Description = "Сертифицированный инструктор по Хатха-йоге и Аштанга-йоге.",
                CreatedAt = DateTime.Now.AddMonths(-6)
            },
            new Trainer
            {
                Id = 2,
                FirstName = "Дмитрий",
                LastName = "Волков",
                Direction = "fitness",
                Email = "dmitry@rockstar.ru",
                PasswordHash = "hashed_2",
                Experience = 8,
                Description = "Эксперт по силовым тренировкам и функциональному фитнесу.",
                CreatedAt = DateTime.Now.AddMonths(-12)
            },
            new Trainer
            {
                Id = 3,
                FirstName = "Елена",
                LastName = "Петрова",
                Direction = "climbing",
                Email = "elena@rockstar.ru",
                PasswordHash = "hashed_3",
                Experience = 6,
                Description = "Мастер спорта по скалолазанию. Проводит тренировки для всех уровней.",
                CreatedAt = DateTime.Now.AddMonths(-3)
            }
        };

        private int _nextId = 4;

        public Task<List<Trainer>> GetAllAsync() =>
            Task.FromResult(_trainers.Where(t => t.IsActive).ToList());

        public Task<Trainer?> GetByIdAsync(int id) =>
            Task.FromResult(_trainers.FirstOrDefault(t => t.Id == id && t.IsActive));

        public Task<bool> CreateAsync(Trainer trainer)
        {
            trainer.Id = _nextId++;
            trainer.CreatedAt = DateTime.Now;
            _trainers.Add(trainer);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Trainer trainer)
        {
            var existing = _trainers.FirstOrDefault(t => t.Id == trainer.Id);
            if (existing != null)
            {
                existing.FirstName = trainer.FirstName;
                existing.LastName = trainer.LastName;
                existing.Direction = trainer.Direction;
                existing.Email = trainer.Email;
                if (!string.IsNullOrEmpty(trainer.PasswordHash))
                    existing.PasswordHash = trainer.PasswordHash;
                existing.Experience = trainer.Experience;
                existing.Description = trainer.Description;
                existing.Photo = trainer.Photo;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var trainer = _trainers.FirstOrDefault(t => t.Id == id);
            if (trainer != null)
            {
                trainer.IsActive = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public List<string> GetAvailableDirections() =>
            new List<string> { "yoga", "fitness", "climbing" };
    }
}