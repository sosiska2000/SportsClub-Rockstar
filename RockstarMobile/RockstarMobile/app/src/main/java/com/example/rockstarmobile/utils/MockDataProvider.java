package com.example.rockstarmobile.utils;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Direction;
import com.example.rockstarmobile.models.Schedule;
import com.example.rockstarmobile.models.Service;
import com.example.rockstarmobile.models.Subscription;
import com.example.rockstarmobile.models.Trainer;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

public class MockDataProvider {

    public static Direction getDirection(String key) {
        switch (key) {
            case "yoga":
                return new Direction(1, "Йога", "yoga",
                        "Практики для гармонии тела и духа. Йога помогает улучшить гибкость, " +
                                "снять стресс и обрести внутренний баланс. В нашем клубе представлены " +
                                "различные направления: Хатха-йога, Аштанга-йога и функциональная йога.");
            case "fitness":
                return new Direction(2, "Фитнес", "fitness",
                        "Тренажерный зал и групповые тренировки для достижения оптимальной " +
                                "физической формы и укрепления здоровья. Современное оборудование, " +
                                "профессиональные тренеры и разнообразные программы тренировок.");
            case "climbing":
                return new Direction(3, "Скалолазание", "climbing",
                        "Скалодром для всех уровней подготовки. Развивает координацию, силу " +
                                "и выносливость. У нас есть трассы разной сложности, боулдеринг, " +
                                "скоростное лазание и индивидуальные занятия с инструктором.");
            default:
                return null;
        }
    }

    public static List<Service> getServices(String directionKey) {
        List<Service> services = new ArrayList<>();

        switch (directionKey) {
            case "yoga":
                services.add(new Service(1, 1, "Разовое посещение", 500, 1, 60, "Одно занятие"));
                services.add(new Service(2, 1, "Абонемент на 8 занятий", 3500, 8, 60, "8 занятий йогой"));
                services.add(new Service(3, 1, "Абонемент на 12 занятий", 4800, 12, 60, "12 занятий йогой"));
                services.add(new Service(4, 1, "Персональное занятие", 1500, 1, 60, "Индивидуальная работа с тренером"));
                services.add(new Service(5, 1, "Групповое занятие", 600, 1, 60, "Занятие в группе"));
                break;
            case "fitness":
                services.add(new Service(6, 2, "Разовое посещение зала", 400, 1, 120, "Тренажерный зал"));
                services.add(new Service(7, 2, "Абонемент на 10 посещений", 3000, 10, 120, "10 посещений"));
                services.add(new Service(8, 2, "Абонемент на месяц", 5000, 30, 120, "Безлимит на месяц"));
                services.add(new Service(9, 2, "Персональная тренировка", 1200, 1, 60, "С тренером"));
                services.add(new Service(10, 2, "Групповая тренировка", 500, 1, 60, "Групповое занятие"));
                break;
            case "climbing":
                services.add(new Service(11, 3, "Разовое посещение", 600, 1, 120, "Скалодром"));
                services.add(new Service(12, 3, "Абонемент на 8 посещений", 4000, 8, 120, "8 посещений"));
                services.add(new Service(13, 3, "Абонемент на 12 посещений", 5500, 12, 120, "12 посещений"));
                services.add(new Service(14, 3, "Персональное занятие", 1800, 1, 90, "С инструктором"));
                services.add(new Service(15, 3, "Групповая тренировка", 700, 1, 90, "Групповое занятие"));
                break;
        }

        return services;
    }

    public static List<Subscription> getSubscriptions(String directionKey) {
        List<Subscription> subscriptions = new ArrayList<>();

        switch (directionKey) {
            case "yoga":
                subscriptions.add(new Subscription(1, "Йога Старт", 1, 3500, 8, "8 занятий йогой"));
                subscriptions.add(new Subscription(2, "Йога Профи", 1, 4800, 12, "12 занятий йогой"));
                break;
            case "fitness":
                subscriptions.add(new Subscription(3, "Фитнес Базовый", 2, 3000, 10, "10 посещений зала"));
                subscriptions.add(new Subscription(4, "Фитнес Безлимит", 2, 5000, 30, "Безлимит на месяц"));
                break;
            case "climbing":
                subscriptions.add(new Subscription(5, "Скалолазание Старт", 3, 4000, 8, "8 посещений"));
                subscriptions.add(new Subscription(6, "Скалолазание Профи", 3, 5500, 12, "12 посещений"));
                break;
        }

        return subscriptions;
    }

    public static List<Integer> getDirectionImages(String directionKey) {
        List<Integer> images = new ArrayList<>();

        // Временные заглушки - замените на реальные изображения
        switch (directionKey) {
            case "yoga":
                images.add(R.drawable.yoga_placeholder);
                images.add(R.drawable.yoga_placeholder);
                images.add(R.drawable.yoga_placeholder);
                break;
            case "fitness":
                images.add(R.drawable.fitness_placeholder);
                images.add(R.drawable.fitness_placeholder);
                images.add(R.drawable.fitness_placeholder);
                break;
            case "climbing":
                images.add(R.drawable.climbing_placeholder);
                images.add(R.drawable.climbing_placeholder);
                images.add(R.drawable.climbing_placeholder);
                break;
        }

        return images;
    }

    public static List<Trainer> getTrainers() {
        List<Trainer> trainers = new ArrayList<>();

        trainers.add(new Trainer(1, "Анна", "Соколова", 1, "Йога", "yoga", 5,
                "Сертифицированный инструктор по Хатха-йоге и Аштанга-йоге. Проводит индивидуальные и групповые занятия."));
        trainers.add(new Trainer(2, "Дмитрий", "Волков", 2, "Фитнес", "fitness", 8,
                "Эксперт по силовым тренировкам и функциональному фитнесу. Мастер спорта по пауэрлифтингу."));
        trainers.add(new Trainer(3, "Елена", "Петрова", 3, "Скалолазание", "climbing", 6,
                "Мастер спорта по скалолазанию. Чемпионка России по боулдерингу. Обучает технике лазания."));
        trainers.add(new Trainer(4, "Алексей", "Иванов", 2, "Фитнес", "fitness", 4,
                "Тренер по функциональному тренингу и кроссфиту. Поможет достичь ваших целей."));
        trainers.add(new Trainer(5, "Мария", "Смирнова", 1, "Йога", "yoga", 3,
                "Инструктор по йоге для начинающих и опытных практиков. Специалист по хатха-йоге."));

        return trainers;
    }

    public static List<Schedule> getSchedule() {
        List<Schedule> schedule = new ArrayList<>();

        // Получаем сегодняшнюю дату в формате yyyy-MM-dd
        Calendar cal = Calendar.getInstance();
        String today = String.format("%d-%02d-%02d",
                cal.get(Calendar.YEAR),
                cal.get(Calendar.MONTH) + 1,
                cal.get(Calendar.DAY_OF_MONTH));

        // Завтра
        cal.add(Calendar.DAY_OF_MONTH, 1);
        String tomorrow = String.format("%d-%02d-%02d",
                cal.get(Calendar.YEAR),
                cal.get(Calendar.MONTH) + 1,
                cal.get(Calendar.DAY_OF_MONTH));

        // Послезавтра
        cal.add(Calendar.DAY_OF_MONTH, 1);
        String dayAfterTomorrow = String.format("%d-%02d-%02d",
                cal.get(Calendar.YEAR),
                cal.get(Calendar.MONTH) + 1,
                cal.get(Calendar.DAY_OF_MONTH));

        schedule.add(new Schedule(1, "Анна Соколова", "Йога", "Хатха-йога",
                today + " 09:00", 60, 15, 5, 600));
        schedule.add(new Schedule(2, "Мария Смирнова", "Йога", "Аштанга-йога",
                today + " 18:00", 90, 12, 8, 700));
        schedule.add(new Schedule(3, "Дмитрий Волков", "Фитнес", "Силовой тренинг",
                today + " 19:00", 60, 20, 12, 500));
        schedule.add(new Schedule(4, "Елена Петрова", "Скалолазание", "Боулдеринг",
                tomorrow + " 16:00", 120, 8, 4, 800));
        schedule.add(new Schedule(5, "Алексей Иванов", "Фитнес", "Функциональный тренинг",
                tomorrow + " 20:00", 60, 15, 10, 500));
        schedule.add(new Schedule(6, "Анна Соколова", "Йога", "Хатха-йога",
                dayAfterTomorrow + " 10:00", 60, 15, 3, 600));
        schedule.add(new Schedule(7, "Елена Петрова", "Скалолазание", "Трудность",
                dayAfterTomorrow + " 15:00", 120, 6, 2, 900));

        return schedule;
    }
}