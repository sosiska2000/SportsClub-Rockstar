package com.example.rockstarmobile.activities;

import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Schedule;
import com.example.rockstarmobile.models.User;
import com.example.rockstarmobile.utils.SessionManager;

public class BookingActivity extends AppCompatActivity {

    private ImageView ivBack;
    private TextView tvDirectionName, tvServiceName, tvTrainerName, tvDateTime, tvDuration;
    private TextView tvPrice, tvParticipants, tvAvailableSpots;
    private Button btnBook, btnCancel;
    private ProgressBar progressBar;

    private Schedule currentSchedule;
    private SessionManager sessionManager;
    private User currentUser;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_booking);

        sessionManager = new SessionManager(this);
        currentUser = sessionManager.getUser();

        // Получаем данные о занятии
        currentSchedule = (Schedule) getIntent().getSerializableExtra("schedule");
        if (currentSchedule == null) {
            Toast.makeText(this, "Ошибка загрузки данных", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        initViews();
        setupData();
        setupListeners();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        tvDirectionName = findViewById(R.id.tvDirectionName);
        tvServiceName = findViewById(R.id.tvServiceName);
        tvTrainerName = findViewById(R.id.tvTrainerName);
        tvDateTime = findViewById(R.id.tvDateTime);
        tvDuration = findViewById(R.id.tvDuration);
        tvPrice = findViewById(R.id.tvPrice);
        tvParticipants = findViewById(R.id.tvParticipants);
        tvAvailableSpots = findViewById(R.id.tvAvailableSpots);
        btnBook = findViewById(R.id.btnBook);
        btnCancel = findViewById(R.id.btnCancel);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupData() {
        tvDirectionName.setText(currentSchedule.getDirectionName());
        tvServiceName.setText(currentSchedule.getServiceName());
        tvTrainerName.setText("Тренер: " + currentSchedule.getTrainerName());
        tvDateTime.setText(currentSchedule.getDateDisplay() + " " + currentSchedule.getTimeRange());
        tvDuration.setText("Длительность: " + currentSchedule.getDurationMinutes() + " мин");
        tvPrice.setText(currentSchedule.getPriceDisplay());
        tvParticipants.setText("Записано: " + currentSchedule.getParticipantsDisplay());

        int available = currentSchedule.getMaxParticipants() - currentSchedule.getCurrentParticipants();
        tvAvailableSpots.setText("Осталось мест: " + available);

        // Если мест нет, блокируем кнопку
        if (available <= 0) {
            btnBook.setEnabled(false);
            btnBook.setText("Мест нет");
            btnBook.setAlpha(0.5f);
        }
    }

    private void setupListeners() {
        ivBack.setOnClickListener(v -> finish());

        btnBook.setOnClickListener(v -> showConfirmationDialog());

        btnCancel.setOnClickListener(v -> finish());
    }

    private void showConfirmationDialog() {
        new AlertDialog.Builder(this)
                .setTitle("Подтверждение записи")
                .setMessage("Вы уверены, что хотите записаться на занятие?\n\n" +
                        currentSchedule.getDirectionName() + " - " + currentSchedule.getServiceName() + "\n" +
                        currentSchedule.getDateDisplay() + " " + currentSchedule.getTimeRange())
                .setPositiveButton("Записаться", (dialog, which) -> performBooking())
                .setNegativeButton("Отмена", null)
                .show();
    }

    private void performBooking() {
        progressBar.setVisibility(View.VISIBLE);
        btnBook.setEnabled(false);
        btnCancel.setEnabled(false);

        // Имитация записи на занятие (API call)
        new Handler().postDelayed(() -> {
            progressBar.setVisibility(View.GONE);
            btnBook.setEnabled(true);
            btnCancel.setEnabled(true);

            // Успешная запись
            Toast.makeText(BookingActivity.this,
                    "Вы успешно записаны на занятие!", Toast.LENGTH_LONG).show();

            // Возвращаемся на предыдущий экран
            finish();
        }, 1500);
    }
}