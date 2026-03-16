package com.example.rockstarmobile.models;

public class Statistic {
    private int totalWorkouts;
    private int totalHours;
    private int totalCalories;
    private int workoutsThisMonth;
    private int enrolledCount;
    private int attendedCount;
    private int cancelledCount;

    public Statistic() {}

    public int getTotalWorkouts() { return totalWorkouts; }
    public void setTotalWorkouts(int totalWorkouts) { this.totalWorkouts = totalWorkouts; }

    public int getTotalHours() { return totalHours; }
    public void setTotalHours(int totalHours) { this.totalHours = totalHours; }

    public int getTotalCalories() { return totalCalories; }
    public void setTotalCalories(int totalCalories) { this.totalCalories = totalCalories; }

    public int getWorkoutsThisMonth() { return workoutsThisMonth; }
    public void setWorkoutsThisMonth(int workoutsThisMonth) { this.workoutsThisMonth = workoutsThisMonth; }

    public int getEnrolledCount() { return enrolledCount; }
    public void setEnrolledCount(int enrolledCount) { this.enrolledCount = enrolledCount; }

    public int getAttendedCount() { return attendedCount; }
    public void setAttendedCount(int attendedCount) { this.attendedCount = attendedCount; }

    public int getCancelledCount() { return cancelledCount; }
    public void setCancelledCount(int cancelledCount) { this.cancelledCount = cancelledCount; }

    public int getAttendanceRate() {
        if (enrolledCount == 0) return 0;
        return (attendedCount * 100) / enrolledCount;
    }
}