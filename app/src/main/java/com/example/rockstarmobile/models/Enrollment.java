package com.example.rockstarmobile.models;

import java.io.Serializable;

public class Enrollment implements Serializable {
    private int id;
    private int userId;
    private String userName;
    private String userEmail;
    private int scheduleId;
    private String scheduleInfo;
    private String enrolledAt;
    private String status; // enrolled, attended, cancelled, no_show
    private String createdAt;

    // Конструкторы
    public Enrollment() {}

    public Enrollment(int id, int userId, String userName, int scheduleId,
                      String enrolledAt, String status) {
        this.id = id;
        this.userId = userId;
        this.userName = userName;
        this.scheduleId = scheduleId;
        this.enrolledAt = enrolledAt;
        this.status = status;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getUserId() { return userId; }
    public void setUserId(int userId) { this.userId = userId; }

    public String getUserName() { return userName; }
    public void setUserName(String userName) { this.userName = userName; }

    public String getUserEmail() { return userEmail; }
    public void setUserEmail(String userEmail) { this.userEmail = userEmail; }

    public int getScheduleId() { return scheduleId; }
    public void setScheduleId(int scheduleId) { this.scheduleId = scheduleId; }

    public String getScheduleInfo() { return scheduleInfo; }
    public void setScheduleInfo(String scheduleInfo) { this.scheduleInfo = scheduleInfo; }

    public String getEnrolledAt() { return enrolledAt; }
    public void setEnrolledAt(String enrolledAt) { this.enrolledAt = enrolledAt; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    public String getStatusDisplay() {
        switch (status) {
            case "enrolled":
                return "Записан";
            case "attended":
                return "Посетил";
            case "cancelled":
                return "Отменен";
            case "no_show":
                return "Не явился";
            default:
                return status;
        }
    }

    public int getStatusColor() {
        switch (status) {
            case "enrolled":
                return 0xFF4CAF50; // зеленый
            case "attended":
                return 0xFF2196F3; // синий
            case "cancelled":
                return 0xFFF44336; // красный
            case "no_show":
                return 0xFFFF9800; // оранжевый
            default:
                return 0xFF9E9E9E; // серый
        }
    }
}