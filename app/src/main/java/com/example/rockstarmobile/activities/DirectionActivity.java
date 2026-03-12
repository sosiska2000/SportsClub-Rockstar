package com.example.rockstarmobile.activities;

import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.adapters.ServicesAdapter;
import com.example.rockstarmobile.adapters.SubscriptionsAdapter;
import com.example.rockstarmobile.models.Direction;
import com.example.rockstarmobile.models.Service;
import com.example.rockstarmobile.models.Subscription;
import com.example.rockstarmobile.utils.MockDataProvider;

import java.util.List;

public class DirectionActivity extends AppCompatActivity {

    private ImageView ivBack, ivDirectionIcon;
    private TextView tvDirectionTitle, tvDirectionDescription;
    private RecyclerView rvServices, rvSubscriptions;
    private ProgressBar progressBar;

    private String directionKey;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_direction);

        // Получаем ключ направления из Intent
        directionKey = getIntent().getStringExtra("direction_key");
        if (directionKey == null || directionKey.isEmpty()) {
            directionKey = "yoga";
        }

        initViews();
        setupListeners();
        loadDirectionData();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        ivDirectionIcon = findViewById(R.id.ivDirectionIcon);
        tvDirectionTitle = findViewById(R.id.tvDirectionTitle);
        tvDirectionDescription = findViewById(R.id.tvDirectionDescription);
        rvServices = findViewById(R.id.rvServices);
        rvSubscriptions = findViewById(R.id.rvSubscriptions);
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
            Direction direction = MockDataProvider.getDirection(directionKey);

            if (direction != null) {
                // Устанавливаем заголовок и описание
                tvDirectionTitle.setText(direction.getName());
                tvDirectionDescription.setText(direction.getDescription());

                // Устанавливаем иконку
                setDirectionIcon();

                // Загружаем услуги
                List<Service> services = MockDataProvider.getServices(directionKey);
                ServicesAdapter servicesAdapter = new ServicesAdapter(this, services);
                rvServices.setAdapter(servicesAdapter);

                // Загружаем абонементы
                List<Subscription> subscriptions = MockDataProvider.getSubscriptions(directionKey);
                SubscriptionsAdapter subscriptionsAdapter = new SubscriptionsAdapter(this, subscriptions);
                rvSubscriptions.setAdapter(subscriptionsAdapter);

            } else {
                Toast.makeText(this, "Направление не найдено", Toast.LENGTH_SHORT).show();
                finish();
            }

            progressBar.setVisibility(View.GONE);
        }, 500);
    }

    private void setDirectionIcon() {
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