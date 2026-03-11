package com.example.rockstarmobile.activities;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.User;
import com.example.rockstarmobile.utils.SessionManager;
import com.example.rockstarmobile.utils.UserDBHelper;

public class LoginActivity extends AppCompatActivity {

    private EditText etEmail, etPassword;
    private Button btnLogin;
    private TextView tvRegister;
    private ProgressBar progressBar;

    private UserDBHelper userDBHelper;
    private SessionManager sessionManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        sessionManager = new SessionManager(this);
        userDBHelper = new UserDBHelper(this);

        // Проверяем, может пользователь уже залогинен
        if (sessionManager.isLoggedIn()) {
            goToMain();
            return;
        }

        initViews();
        setupHintClearListeners();
        setupListeners();
    }

    private void initViews() {
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        btnLogin = findViewById(R.id.btnLogin);
        tvRegister = findViewById(R.id.tvRegister);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupHintClearListeners() {
        etEmail.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus && etEmail.getText().toString().trim().equals("Введите email")) {
                etEmail.setText("");
            } else if (!hasFocus && etEmail.getText().toString().trim().isEmpty()) {
                etEmail.setText("Введите email");
            }
        });

        etPassword.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus && etPassword.getText().toString().trim().equals("Введите пароль")) {
                etPassword.setText("");
            } else if (!hasFocus && etPassword.getText().toString().trim().isEmpty()) {
                etPassword.setText("Введите пароль");
            }
        });
    }

    private void setupListeners() {
        btnLogin.setOnClickListener(v -> attemptLogin());
        tvRegister.setOnClickListener(v -> {
            Intent intent = new Intent(LoginActivity.this, RegisterActivity.class);
            startActivity(intent);
        });
    }

    private void attemptLogin() {
        String email = etEmail.getText().toString().trim();
        String password = etPassword.getText().toString().trim();

        if (email.equals("Введите email") || TextUtils.isEmpty(email)) {
            etEmail.setError("Введите email");
            return;
        }
        if (password.equals("Введите пароль") || TextUtils.isEmpty(password)) {
            etPassword.setError("Введите пароль");
            return;
        }

        progressBar.setVisibility(View.VISIBLE);
        btnLogin.setEnabled(false);

        // Пытаемся авторизоваться через БД
        userDBHelper.loginUser(email, password, new UserDBHelper.UserCallback() {
            @Override
            public void onSuccess(User user) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnLogin.setEnabled(true);

                    sessionManager.saveUser(user);
                    Toast.makeText(LoginActivity.this,
                            "Добро пожаловать, " + user.getFirstName() + "!",
                            Toast.LENGTH_SHORT).show();
                    goToMain();
                });
            }

            @Override
            public void onError(String error) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnLogin.setEnabled(true);
                    Toast.makeText(LoginActivity.this,
                            "Ошибка входа: " + error, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void goToMain() {
        Intent intent = new Intent(LoginActivity.this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}