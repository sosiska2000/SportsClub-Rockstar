package com.example.rockstarmobile.models;

import java.io.Serializable;

public class User implements Serializable {
    private int id;
    private String firstName;
    private String lastName;
    private int age;
    private String phone;
    private String email;
    private String passwordHash;
    private byte[] photo;
    private String createdAt;
    private boolean isActive;
    private String role;

    // Конструкторы
    public User() {}

    public User(String firstName, String lastName, String email, String password) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.passwordHash = password;
    }

    public User(int id, String firstName, String lastName, int age, String phone,
                String email, String createdAt, boolean isActive, String role) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.age = age;
        this.phone = phone;
        this.email = email;
        this.createdAt = createdAt;
        this.isActive = isActive;
        this.role = role;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getFirstName() { return firstName; }
    public void setFirstName(String firstName) { this.firstName = firstName; }

    public String getLastName() { return lastName; }
    public void setLastName(String lastName) { this.lastName = lastName; }

    public int getAge() { return age; }
    public void setAge(int age) { this.age = age; }

    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getPasswordHash() { return passwordHash; }
    public void setPasswordHash(String passwordHash) { this.passwordHash = passwordHash; }

    public byte[] getPhoto() { return photo; }
    public void setPhoto(byte[] photo) { this.photo = photo; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getRole() { return role; }
    public void setRole(String role) { this.role = role; }

    public String getFullName() {
        if (firstName == null && lastName == null) return "Пользователь";
        if (firstName == null) return lastName;
        if (lastName == null) return firstName;
        return firstName + " " + lastName;
    }

    public String getFormattedDate() {
        if (createdAt == null || createdAt.isEmpty()) {
            return "—";
        }
        try {
            if (createdAt.contains("T")) {
                String datePart = createdAt.split("T")[0];
                String timePart = createdAt.split("T")[1].split("\\.")[0];
                return datePart + " " + timePart;
            } else {
                return createdAt;
            }
        } catch (Exception e) {
            return createdAt;
        }
    }
}