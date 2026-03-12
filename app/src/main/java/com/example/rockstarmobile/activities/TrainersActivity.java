package com.example.rockstarmobile.activities;

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
import com.example.rockstarmobile.adapters.TrainersAdapter;
import com.example.rockstarmobile.models.Trainer;
import com.example.rockstarmobile.utils.MockDataProvider;

import java.util.ArrayList;
import java.util.List;

public class TrainersActivity extends AppCompatActivity {

    private ImageView ivBack;
    private TextView tvEmptyState;
    private RecyclerView rvTrainers;
    private ProgressBar progressBar;
    private Button btnRetry;

    // Кнопки фильтров
    private Button btnAll, btnYoga, btnFitness, btnClimbing;
    private Button selectedFilterButton;

    private List<Trainer> allTrainers = new ArrayList<>();
    private String currentFilter = "all";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_trainers);

        initViews();
        setupListeners();
        loadTrainers();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        tvEmptyState = findViewById(R.id.tvEmptyState);
        rvTrainers = findViewById(R.id.rvTrainers);
        progressBar = findViewById(R.id.progressBar);
        btnRetry = findViewById(R.id.btnRetry);

        // Инициализация кнопок фильтров
        btnAll = findViewById(R.id.btnAll);
        btnYoga = findViewById(R.id.btnYoga);
        btnFitness = findViewById(R.id.btnFitness);
        btnClimbing = findViewById(R.id.btnClimbing);

        rvTrainers.setLayoutManager(new LinearLayoutManager(this));

        // Устанавливаем текст для кнопок
        if (btnAll != null) btnAll.setText("Все");
        if (btnYoga != null) btnYoga.setText("Йога");
        if (btnFitness != null) btnFitness.setText("Фитнес");
        if (btnClimbing != null) btnClimbing.setText("Скалолазание");
    }

    private void setupListeners() {
        ivBack.setOnClickListener(v -> finish());
        btnRetry.setOnClickListener(v -> loadTrainers());

        btnAll.setOnClickListener(v -> selectFilter(btnAll, "all"));
        btnYoga.setOnClickListener(v -> selectFilter(btnYoga, "yoga"));
        btnFitness.setOnClickListener(v -> selectFilter(btnFitness, "fitness"));
        btnClimbing.setOnClickListener(v -> selectFilter(btnClimbing, "climbing"));

        // По умолчанию выбираем "Все"
        selectFilter(btnAll, "all");
    }

    private void selectFilter(Button button, String filter) {
        // Сбрасываем стиль предыдущей выбранной кнопки
        if (selectedFilterButton != null) {
            selectedFilterButton.setBackgroundResource(R.drawable.btn_outline_small);
            // Используем ContextCompat для поддержки старых версий Android
            selectedFilterButton.setTextColor(ContextCompat.getColor(this, R.color.text_secondary));
        }

        // Устанавливаем стиль для выбранной кнопки
        button.setBackgroundResource(R.drawable.btn_primary_small);
        button.setTextColor(ContextCompat.getColor(this, android.R.color.white));
        selectedFilterButton = button;

        currentFilter = filter;
        filterTrainers();
    }

    private void loadTrainers() {
        progressBar.setVisibility(View.VISIBLE);
        rvTrainers.setVisibility(View.GONE);
        tvEmptyState.setVisibility(View.GONE);
        btnRetry.setVisibility(View.GONE);

        new Handler().postDelayed(() -> {
            allTrainers = MockDataProvider.getTrainers();
            progressBar.setVisibility(View.GONE);
            filterTrainers();
        }, 1000);
    }

    private void filterTrainers() {
        List<Trainer> filteredList = new ArrayList<>();

        if (currentFilter.equals("all")) {
            filteredList.addAll(allTrainers);
        } else {
            for (Trainer trainer : allTrainers) {
                String directionKey = trainer.getDirectionKey();
                if (directionKey != null && directionKey.equals(currentFilter)) {
                    filteredList.add(trainer);
                }
            }
        }

        if (filteredList.isEmpty()) {
            tvEmptyState.setVisibility(View.VISIBLE);
            rvTrainers.setVisibility(View.GONE);
            btnRetry.setVisibility(View.VISIBLE);
        } else {
            tvEmptyState.setVisibility(View.GONE);
            rvTrainers.setVisibility(View.VISIBLE);
            btnRetry.setVisibility(View.GONE);
            TrainersAdapter adapter = new TrainersAdapter(TrainersActivity.this, filteredList);
            rvTrainers.setAdapter(adapter);
        }
    }
}