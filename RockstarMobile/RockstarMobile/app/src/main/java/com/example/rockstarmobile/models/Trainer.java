package com.example.rockstarmobile.models;

import java.io.Serializable;

public class Trainer implements Serializable {
    private int id;
    private String firstName;
    private String lastName;
    private Integer directionId;
    private String directionName;
    private String directionKey;
    private String email;
    private String passwordHash;
    private byte[] photo;
    private int experience;
    private String description;
    private boolean isActive;
    private String createdAt;

    // Конструкторы
    public Trainer() {}

    public Trainer(int id, String firstName, String lastName, Integer directionId,
                   String directionName, String directionKey, int experience, String description) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.directionId = directionId;
        this.directionName = directionName;
        this.directionKey = directionKey;
        this.experience = experience;
        this.description = description;
        this.isActive = true;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getFirstName() { return firstName; }
    public void setFirstName(String firstName) { this.firstName = firstName; }

    public String getLastName() { return lastName; }
    public void setLastName(String lastName) { this.lastName = lastName; }

    public Integer getDirectionId() { return directionId; }
    public void setDirectionId(Integer directionId) { this.directionId = directionId; }

    public String getDirectionName() { return directionName; }
    public void setDirectionName(String directionName) { this.directionName = directionName; }

    public String getDirectionKey() { return directionKey; }
    public void setDirectionKey(String directionKey) { this.directionKey = directionKey; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getPasswordHash() { return passwordHash; }
    public void setPasswordHash(String passwordHash) { this.passwordHash = passwordHash; }

    public byte[] getPhoto() { return photo; }
    public void setPhoto(byte[] photo) { this.photo = photo; }

    public int getExperience() { return experience; }
    public void setExperience(int experience) { this.experience = experience; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    public String getFullName() {
        if (firstName == null && lastName == null) return "Тренер";
        if (firstName == null) return lastName;
        if (lastName == null) return firstName;
        return firstName + " " + lastName;
    }

    public String getExperienceDisplay() {
        return "Стаж: " + experience + " " + getExperienceWord(experience);
    }

    private String getExperienceWord(int years) {
        if (years % 10 == 1 && years % 100 != 11) {
            return "год";
        } else if (years % 10 >= 2 && years % 10 <= 4 && (years % 100 < 10 || years % 100 >= 20)) {
            return "года";
        } else {
            return "лет";
        }
    }
}