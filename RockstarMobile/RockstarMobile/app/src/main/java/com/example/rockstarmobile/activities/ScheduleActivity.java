package com.example.rockstarmobile.activities;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.adapters.ScheduleAdapter;
import com.example.rockstarmobile.models.Schedule;
import com.example.rockstarmobile.utils.MockDataProvider;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class ScheduleActivity extends AppCompatActivity {

    private ImageView ivBack;
    private TextView tvEmptyState;
    private RecyclerView rvSchedule;
    private ProgressBar progressBar;
    private Button btnRetry;

    // Кнопки для дней
    private Button btnDay1, btnDay2, btnDay3, btnDay4, btnDay5;
    private Button selectedDateButton;

    private String selectedDate;
    private ScheduleAdapter adapter;
    private List<Schedule> allScheduleList = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_schedule);

        initViews();
        generateDateButtons();
        setupListeners();
        loadAllSchedule();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        tvEmptyState = findViewById(R.id.tvEmptyState);
        rvSchedule = findViewById(R.id.rvSchedule);
        progressBar = findViewById(R.id.progressBar);
        btnRetry = findViewById(R.id.btnRetry);

        // Инициализация кнопок дней - ЭТИ ID ДОЛЖНЫ БЫТЬ В XML
        btnDay1 = findViewById(R.id.btnDay1);
        btnDay2 = findViewById(R.id.btnDay2);
        btnDay3 = findViewById(R.id.btnDay3);
        btnDay4 = findViewById(R.id.btnDay4);
        btnDay5 = findViewById(R.id.btnDay5);

        rvSchedule.setLayoutManager(new LinearLayoutManager(this));
    }

    private void generateDateButtons() {
        SimpleDateFormat sdf = new SimpleDateFormat("dd MMM", Locale.getDefault());
        Calendar calendar = Calendar.getInstance();

        // Сегодня
        btnDay1.setText(sdf.format(calendar.getTime()));
        btnDay1.setTag(getDateString(calendar.getTime()));

        // Завтра
        calendar.add(Calendar.DAY_OF_MONTH, 1);
        btnDay2.setText(sdf.format(calendar.getTime()));
        btnDay2.setTag(getDateString(calendar.getTime()));

        // Послезавтра
        calendar.add(Calendar.DAY_OF_MONTH, 1);
        btnDay3.setText(sdf.format(calendar.getTime()));
        btnDay3.setTag(getDateString(calendar.getTime()));

        // +3 дня
        calendar.add(Calendar.DAY_OF_MONTH, 1);
        btnDay4.setText(sdf.format(calendar.getTime()));
        btnDay4.setTag(getDateString(calendar.getTime()));

        // +4 дня
        calendar.add(Calendar.DAY_OF_MONTH, 1);
        btnDay5.setText(sdf.format(calendar.getTime()));
        btnDay5.setTag(getDateString(calendar.getTime()));
    }

    private String getDateString(Date date) {
        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
        return sdf.format(date);
    }

    private void setupListeners() {
        ivBack.setOnClickListener(v -> finish());
        btnRetry.setOnClickListener(v -> loadAllSchedule());

        btnDay1.setOnClickListener(v -> selectDate(btnDay1));
        btnDay2.setOnClickListener(v -> selectDate(btnDay2));
        btnDay3.setOnClickListener(v -> selectDate(btnDay3));
        btnDay4.setOnClickListener(v -> selectDate(btnDay4));
        btnDay5.setOnClickListener(v -> selectDate(btnDay5));

        // По умолчанию выбираем сегодня
        selectDate(btnDay1);
    }

    private void selectDate(Button button) {
        // Сбрасываем стиль предыдущей выбранной кнопки
        if (selectedDateButton != null) {
            selectedDateButton.setBackgroundResource(R.drawable.btn_outline_small);
            selectedDateButton.setTextColor(ContextCompat.getColor(this, R.color.text_secondary));
        }

        // Устанавливаем стиль для выбранной кнопки
        button.setBackgroundResource(R.drawable.btn_primary_small);
        button.setTextColor(ContextCompat.getColor(this, android.R.color.white));
        selectedDateButton = button;

        selectedDate = (String) button.getTag();
        filterScheduleByDate();
    }

    private void loadAllSchedule() {
        progressBar.setVisibility(View.VISIBLE);
        rvSchedule.setVisibility(View.GONE);
        tvEmptyState.setVisibility(View.GONE);
        btnRetry.setVisibility(View.GONE);

        new Handler().postDelayed(() -> {
            allScheduleList = MockDataProvider.getSchedule();
            progressBar.setVisibility(View.GONE);
            filterScheduleByDate();
        }, 1000);
    }

    private void filterScheduleByDate() {
        if (allScheduleList.isEmpty()) {
            tvEmptyState.setVisibility(View.VISIBLE);
            btnRetry.setVisibility(View.VISIBLE);
            return;
        }

        List<Schedule> filteredList = new ArrayList<>();
        for (Schedule schedule : allScheduleList) {
            if (schedule.getDateDisplay().equals(selectedDate)) {
                filteredList.add(schedule);
            }
        }

        if (filteredList.isEmpty()) {
            tvEmptyState.setVisibility(View.VISIBLE);
            rvSchedule.setVisibility(View.GONE);
        } else {
            tvEmptyState.setVisibility(View.GONE);
            rvSchedule.setVisibility(View.VISIBLE);
            adapter = new ScheduleAdapter(ScheduleActivity.this, filteredList,
                    schedule -> {
                        Intent intent = new Intent(ScheduleActivity.this, BookingActivity.class);
                        intent.putExtra("schedule", schedule);
                        startActivity(intent);
                    });
            rvSchedule.setAdapter(adapter);
        }
    }
}