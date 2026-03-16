package com.example.rockstarmobile.models;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class Schedule implements Serializable {
    private int id;
    private int trainerId;
    private String trainerName;
    private int directionId;
    private String directionName;
    private Integer serviceId;
    private String serviceName;
    private String dateTime;
    private int durationMinutes;
    private int maxParticipants;
    private int currentParticipants;
    private double price;
    private boolean isGroup;
    private boolean isActive;
    private String createdAt;

    // Конструкторы
    public Schedule() {}

    public Schedule(int id, String trainerName, String directionName, String serviceName,
                    String dateTime, int durationMinutes, int maxParticipants,
                    int currentParticipants, double price) {
        this.id = id;
        this.trainerName = trainerName;
        this.directionName = directionName;
        this.serviceName = serviceName;
        this.dateTime = dateTime;
        this.durationMinutes = durationMinutes;
        this.maxParticipants = maxParticipants;
        this.currentParticipants = currentParticipants;
        this.price = price;
        this.isGroup = true;
        this.isActive = true;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getTrainerId() { return trainerId; }
    public void setTrainerId(int trainerId) { this.trainerId = trainerId; }

    public String getTrainerName() { return trainerName; }
    public void setTrainerName(String trainerName) { this.trainerName = trainerName; }

    public int getDirectionId() { return directionId; }
    public void setDirectionId(int directionId) { this.directionId = directionId; }

    public String getDirectionName() { return directionName; }
    public void setDirectionName(String directionName) { this.directionName = directionName; }

    public Integer getServiceId() { return serviceId; }
    public void setServiceId(Integer serviceId) { this.serviceId = serviceId; }

    public String getServiceName() { return serviceName; }
    public void setServiceName(String serviceName) { this.serviceName = serviceName; }

    public String getDateTime() { return dateTime; }
    public void setDateTime(String dateTime) { this.dateTime = dateTime; }

    public int getDurationMinutes() { return durationMinutes; }
    public void setDurationMinutes(int durationMinutes) { this.durationMinutes = durationMinutes; }

    public int getMaxParticipants() { return maxParticipants; }
    public void setMaxParticipants(int maxParticipants) { this.maxParticipants = maxParticipants; }

    public int getCurrentParticipants() { return currentParticipants; }
    public void setCurrentParticipants(int currentParticipants) { this.currentParticipants = currentParticipants; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public boolean isGroup() { return isGroup; }
    public void setGroup(boolean group) { isGroup = group; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    // Вспомогательные методы
    public String getDateDisplay() {
        if (dateTime == null) return "";
        if (dateTime.contains(" ")) {
            return dateTime.split(" ")[0];
        }
        return dateTime;
    }

    public String getTimeDisplay() {
        if (dateTime == null) return "";
        if (dateTime.contains(" ")) {
            String[] parts = dateTime.split(" ");
            if (parts.length > 1) {
                return parts[1];
            }
        }
        return "";
    }

    public String getTimeRange() {
        String startTime = getTimeDisplay();
        if (startTime.isEmpty()) return "";

        String[] timeParts = startTime.split(":");
        if (timeParts.length < 2) return startTime;

        try {
            int hours = Integer.parseInt(timeParts[0]);
            int minutes = Integer.parseInt(timeParts[1]);

            int totalMinutes = hours * 60 + minutes + durationMinutes;
            int endHours = (totalMinutes / 60) % 24;
            int endMinutes = totalMinutes % 60;

            String endTime = String.format("%02d:%02d", endHours, endMinutes);
            return startTime + " - " + endTime;
        } catch (NumberFormatException e) {
            return startTime;
        }
    }

    public String getParticipantsDisplay() {
        return currentParticipants + "/" + maxParticipants;
    }

    public String getPriceDisplay() {
        return String.format("%.0f ₽", price);
    }

    public boolean isAvailable() {
        return currentParticipants < maxParticipants;
    }

    public int getAvailableSpots() {
        return maxParticipants - currentParticipants;
    }
}