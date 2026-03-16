package com.example.rockstarmobile.activities;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.viewpager2.widget.ViewPager2;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.adapters.ServicesAdapter;
import com.example.rockstarmobile.adapters.SubscriptionsAdapter;
import com.example.rockstarmobile.adapters.ImageSliderAdapter;
import com.example.rockstarmobile.models.Direction;
import com.example.rockstarmobile.models.Service;
import com.example.rockstarmobile.models.Subscription;
import com.example.rockstarmobile.utils.MockDataProvider;

import java.util.List;

public class DirectionActivity extends AppCompatActivity {

    private ImageView ivBack, ivDirectionIcon;
    private TextView tvDirectionTitle, tvDirectionDescription;
    private RecyclerView rvServices, rvSubscriptions;
    private ViewPager2 viewPagerSlider;
    private ProgressBar progressBar;

    private String directionKey;
    private Direction currentDirection;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_direction);

        // Получаем ключ направления из Intent
        directionKey = getIntent().getStringExtra("direction_key");
        if (directionKey == null || directionKey.isEmpty()) {
            directionKey = "yoga"; // По умолчанию йога
        }

        initViews();
        loadDirectionData();
        setupListeners();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        ivDirectionIcon = findViewById(R.id.ivDirectionIcon);
        tvDirectionTitle = findViewById(R.id.tvDirectionTitle);
        tvDirectionDescription = findViewById(R.id.tvDirectionDescription);
        rvServices = findViewById(R.id.rvServices);
        rvSubscriptions = findViewById(R.id.rvSubscriptions);
        viewPagerSlider = findViewById(R.id.viewPagerSlider);
        progressBar = findViewById(R.id.progressBar);

        // Настройка RecyclerView
        rvServices.setLayoutManager(new LinearLayoutManager(this));
        rvSubscriptions.setLayoutManager(new LinearLayoutManager(this));
    }

    private void setupListeners() {
        ivBack.setOnClickListener(v -> finish());
    }

    private void loadDirectionData() {
        progressBar.setVisibility(View.VISIBLE);

        // Имитация загрузки данных
        new Handler().postDelayed(() -> {
            currentDirection = MockDataProvider.getDirection(directionKey);

            if (currentDirection != null) {
                // Устанавливаем заголовок и описание
                tvDirectionTitle.setText(currentDirection.getName());
                tvDirectionDescription.setText(currentDirection.getDescription());

                // Устанавливаем иконку в зависимости от направления
                setDirectionIcon();

                // Загружаем услуги
                List<Service> services = MockDataProvider.getServices(directionKey);
                ServicesAdapter servicesAdapter = new ServicesAdapter(this, services);
                rvServices.setAdapter(servicesAdapter);

                // Загружаем абонементы
                List<Subscription> subscriptions = MockDataProvider.getSubscriptions(directionKey);
                SubscriptionsAdapter subscriptionsAdapter = new SubscriptionsAdapter(this, subscriptions);
                rvSubscriptions.setAdapter(subscriptionsAdapter);

                // Загружаем слайдер с фото
                List<Integer> images = MockDataProvider.getDirectionImages(directionKey);
                if (images != null && !images.isEmpty()) {
                    ImageSliderAdapter sliderAdapter = new ImageSliderAdapter(this, images);
                    viewPagerSlider.setAdapter(sliderAdapter);
                }
            } else {
                Toast.makeText(this, "Направление не найдено", Toast.LENGTH_SHORT).show();
                finish();
            }

            progressBar.setVisibility(View.GONE);
        }, 500);
    }

    private void setDirectionIcon() {
        if (ivDirectionIcon == null) return;

        switch (directionKey) {
            case "yoga":
                ivDirectionIcon.setImageResource(R.drawable.ic_yoga);
                break;
            case "fitness":
                ivDirectionIcon.setImageResource(R.drawable.ic_fitness);
                break;
            case "climbing":
                ivDirectionIcon.setImageResource(R.drawable.ic_climbing);
                break;
            default:
                ivDirectionIcon.setImageResource(R.drawable.ic_yoga);
                break;
        }
    }
}