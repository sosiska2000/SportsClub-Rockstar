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

public class RegisterActivity extends AppCompatActivity {

    private EditText etLastName, etFirstName, etAge, etPhone, etEmail, etPassword, etConfirmPassword;
    private Button btnRegister;
    private TextView tvLogin;
    private ProgressBar progressBar;

    private UserDBHelper userDBHelper;
    private SessionManager sessionManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        sessionManager = new SessionManager(this);
        userDBHelper = new UserDBHelper(this);

        initViews();
        setupHintClearListeners();
        setupListeners();
    }

    private void initViews() {
        etLastName = findViewById(R.id.etLastName);
        etFirstName = findViewById(R.id.etFirstName);
        etAge = findViewById(R.id.etAge);
        etPhone = findViewById(R.id.etPhone);
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        etConfirmPassword = findViewById(R.id.etConfirmPassword);
        btnRegister = findViewById(R.id.btnRegister);
        tvLogin = findViewById(R.id.tvLogin);
        progressBar = findViewById(R.id.progressBar);

        // Устанавливаем начальные подсказки
        etLastName.setHint("Введите фамилию");
        etFirstName.setHint("Введите имя");
        etAge.setHint("Введите возраст");
        etPhone.setHint("Введите телефон");
        etEmail.setHint("Введите email");
        etPassword.setHint("Введите пароль");
        etConfirmPassword.setHint("Повторите пароль");
    }

    private void setupHintClearListeners() {
        setHintClearListener(etLastName, "Введите фамилию");
        setHintClearListener(etFirstName, "Введите имя");
        setHintClearListener(etAge, "Введите возраст");
        setHintClearListener(etPhone, "Введите телефон");
        setHintClearListener(etEmail, "Введите email");
        setHintClearListener(etPassword, "Введите пароль");
        setHintClearListener(etConfirmPassword, "Повторите пароль");
    }

    private void setHintClearListener(EditText editText, String hint) {
        editText.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus) {
                if (editText.getText().toString().trim().equals(hint)) {
                    editText.setText("");
                }
            } else {
                if (editText.getText().toString().trim().isEmpty()) {
                    editText.setText(hint);
                }
            }
        });
    }

    private void setupListeners() {
        btnRegister.setOnClickListener(v -> attemptRegister());
        tvLogin.setOnClickListener(v -> {
            Intent intent = new Intent(RegisterActivity.this, LoginActivity.class);
            startActivity(intent);
            finish();
        });
    }

    private void attemptRegister() {
        String lastName = etLastName.getText().toString().trim();
        String firstName = etFirstName.getText().toString().trim();
        String ageStr = etAge.getText().toString().trim();
        String phone = etPhone.getText().toString().trim();
        String email = etEmail.getText().toString().trim();
        String password = etPassword.getText().toString().trim();
        String confirmPassword = etConfirmPassword.getText().toString().trim();

        // Валидация
        if (TextUtils.isEmpty(lastName) || lastName.equals("Введите фамилию")) {
            etLastName.setError("Введите фамилию");
            return;
        }
        if (TextUtils.isEmpty(firstName) || firstName.equals("Введите имя")) {
            etFirstName.setError("Введите имя");
            return;
        }
        if (TextUtils.isEmpty(phone) || phone.equals("Введите телефон")) {
            etPhone.setError("Введите телефон");
            return;
        }
        if (TextUtils.isEmpty(email) || email.equals("Введите email")) {
            etEmail.setError("Введите email");
            return;
        }
        if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            etEmail.setError("Введите корректный email");
            return;
        }
        if (password.length() < 6) {
            etPassword.setError("Пароль должен быть не менее 6 символов");
            return;
        }
        if (!password.equals(confirmPassword)) {
            etConfirmPassword.setError("Пароли не совпадают");
            return;
        }

        final int age;
        if (!TextUtils.isEmpty(ageStr) && !ageStr.equals("Введите возраст")) {
            try {
                age = Integer.parseInt(ageStr);
            } catch (NumberFormatException e) {
                etAge.setError("Некорректный возраст");
                return;
            }
        } else {
            age = 0;
        }

        progressBar.setVisibility(View.VISIBLE);
        btnRegister.setEnabled(false);

        // Проверяем, не занят ли email
        userDBHelper.checkEmailExists(email, new UserDBHelper.BooleanCallback() {
            @Override
            public void onSuccess(boolean exists) {
                if (exists) {
                    runOnUiThread(() -> {
                        progressBar.setVisibility(View.GONE);
                        btnRegister.setEnabled(true);
                        etEmail.setError("Этот email уже зарегистрирован");
                    });
                } else {
                    // Регистрируем нового пользователя
                    User user = new User();
                    user.setFirstName(firstName);
                    user.setLastName(lastName);
                    user.setAge(age);
                    user.setPhone(phone);
                    user.setEmail(email);
                    user.setPasswordHash(password); // В реальном проекте нужно хешировать!

                    userDBHelper.registerUser(user, new UserDBHelper.BooleanCallback() {
                        @Override
                        public void onSuccess(boolean success) {
                            runOnUiThread(() -> {
                                progressBar.setVisibility(View.GONE);
                                btnRegister.setEnabled(true);

                                if (success) {
                                    // После регистрации сразу логиним пользователя
                                    userDBHelper.loginUser(email, password, new UserDBHelper.UserCallback() {
                                        @Override
                                        public void onSuccess(User loggedInUser) {
                                            runOnUiThread(() -> {
                                                sessionManager.saveUser(loggedInUser);
                                                Toast.makeText(RegisterActivity.this,
                                                        "Регистрация успешна!", Toast.LENGTH_SHORT).show();

                                                Intent intent = new Intent(RegisterActivity.this, MainActivity.class);
                                                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                                                startActivity(intent);
                                                finish();
                                            });
                                        }

                                        @Override
                                        public void onError(String error) {
                                            runOnUiThread(() -> {
                                                Toast.makeText(RegisterActivity.this,
                                                        "Регистрация успешна, но ошибка входа", Toast.LENGTH_SHORT).show();
                                                finish();
                                            });
                                        }
                                    });
                                } else {
                                    Toast.makeText(RegisterActivity.this,
                                            "Ошибка при регистрации", Toast.LENGTH_SHORT).show();
                                }
                            });
                        }

                        @Override
                        public void onError(String error) {
                            runOnUiThread(() -> {
                                progressBar.setVisibility(View.GONE);
                                btnRegister.setEnabled(true);
                                Toast.makeText(RegisterActivity.this,
                                        "Ошибка: " + error, Toast.LENGTH_LONG).show();
                            });
                        }
                    });
                }
            }

            @Override
            public void onError(String error) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnRegister.setEnabled(true);
                    Toast.makeText(RegisterActivity.this,
                            "Ошибка проверки email: " + error, Toast.LENGTH_LONG).show();
                });
            }
        });
    }
}