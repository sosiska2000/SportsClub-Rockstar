package com.example.rockstarmobile.activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.User;
import com.example.rockstarmobile.utils.SessionManager;

public class MainActivity extends AppCompatActivity {

    private CardView cardYoga, cardFitness, cardClimbing, cardSchedule, cardTrainers, cardProfile;
    private Button btnLogout;
    private TextView tvWelcome;

    private SessionManager sessionManager;
    private User currentUser;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        sessionManager = new SessionManager(this);
        currentUser = sessionManager.getUser();

        initViews();
        setupListeners();
        setupUserInfo();
    }

    private void initViews() {
        cardYoga = findViewById(R.id.cardYoga);
        cardFitness = findViewById(R.id.cardFitness);
        cardClimbing = findViewById(R.id.cardClimbing);
        cardSchedule = findViewById(R.id.cardSchedule);
        cardTrainers = findViewById(R.id.cardTrainers);
        cardProfile = findViewById(R.id.cardProfile);
        btnLogout = findViewById(R.id.btnLogout);
        tvWelcome = findViewById(R.id.tvWelcome);
    }

    private void setupListeners() {
        cardYoga.setOnClickListener(v -> openDirection("yoga"));
        cardFitness.setOnClickListener(v -> openDirection("fitness"));
        cardClimbing.setOnClickListener(v -> openDirection("climbing"));
        cardSchedule.setOnClickListener(v -> openSchedule());
        cardTrainers.setOnClickListener(v -> openTrainers());
        cardProfile.setOnClickListener(v -> openProfile());
        btnLogout.setOnClickListener(v -> logout());
    }

    private void setupUserInfo() {
        if (currentUser != null) {
            tvWelcome.setText("Привет, " + currentUser.getFirstName() + "!");
        } else {
            tvWelcome.setText("Привет, Гость!");
        }
    }

    private void openDirection(String directionKey) {
        Intent intent = new Intent(MainActivity.this, DirectionActivity.class);
        intent.putExtra("direction_key", directionKey);
        startActivity(intent);
    }

    private void openSchedule() {
        Intent intent = new Intent(MainActivity.this, ScheduleActivity.class);
        startActivity(intent);
    }

    private void openTrainers() {
        Intent intent = new Intent(MainActivity.this, TrainersActivity.class);
        startActivity(intent);
    }

    private void openProfile() {
        Intent intent = new Intent(MainActivity.this, ProfileActivity.class);
        startActivity(intent);
    }

    private void logout() {
        sessionManager.logout();
        Intent intent = new Intent(MainActivity.this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}