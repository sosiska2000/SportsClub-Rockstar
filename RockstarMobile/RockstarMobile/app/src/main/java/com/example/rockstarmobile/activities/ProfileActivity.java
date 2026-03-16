package com.example.rockstarmobile.activities;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.provider.MediaStore;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.User;
import com.example.rockstarmobile.utils.SessionManager;

import java.io.ByteArrayOutputStream;
import java.io.IOException;

public class ProfileActivity extends AppCompatActivity {

    private static final int PICK_IMAGE_REQUEST = 1;

    private ImageView ivBack, ivProfilePhoto, ivEditPhoto;
    private TextView tvEmail;
    private EditText etFirstName, etLastName, etAge, etPhone;
    private Button btnSave, btnChangePassword, btnLogout;
    private ProgressBar progressBar;

    private SessionManager sessionManager;
    private User currentUser;
    private byte[] newPhotoData;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        sessionManager = new SessionManager(this);
        currentUser = sessionManager.getUser();

        initViews();
        setupData();
        setupListeners();
    }

    private void initViews() {
        ivBack = findViewById(R.id.ivBack);
        ivProfilePhoto = findViewById(R.id.ivProfilePhoto);
        ivEditPhoto = findViewById(R.id.ivEditPhoto);
        tvEmail = findViewById(R.id.tvEmail);
        etFirstName = findViewById(R.id.etFirstName);
        etLastName = findViewById(R.id.etLastName);
        etAge = findViewById(R.id.etAge);
        etPhone = findViewById(R.id.etPhone);
        btnSave = findViewById(R.id.btnSave);
        btnChangePassword = findViewById(R.id.btnChangePassword);
        btnLogout = findViewById(R.id.btnLogout);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupData() {
        if (currentUser != null) {
            tvEmail.setText(currentUser.getEmail());
            etFirstName.setText(currentUser.getFirstName());
            etLastName.setText(currentUser.getLastName());
            if (currentUser.getAge() > 0) {
                etAge.setText(String.valueOf(currentUser.getAge()));
            }
            etPhone.setText(currentUser.getPhone());

            // Загрузка фото (если есть)
            if (currentUser.getPhoto() != null) {
                Bitmap bitmap = BitmapFactory.decodeByteArray(currentUser.getPhoto(), 0, currentUser.getPhoto().length);
                ivProfilePhoto.setImageBitmap(bitmap);
            }
        }
    }

    private void setupListeners() {
        ivBack.setOnClickListener(v -> finish());

        ivEditPhoto.setOnClickListener(v -> openImagePicker());

        btnSave.setOnClickListener(v -> saveProfile());

        btnChangePassword.setOnClickListener(v -> showChangePasswordDialog());

        btnLogout.setOnClickListener(v -> logout());
    }

    private void openImagePicker() {
        Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
        startActivityForResult(intent, PICK_IMAGE_REQUEST);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == PICK_IMAGE_REQUEST && resultCode == RESULT_OK && data != null) {
            Uri imageUri = data.getData();
            try {
                Bitmap bitmap = MediaStore.Images.Media.getBitmap(this.getContentResolver(), imageUri);
                ivProfilePhoto.setImageBitmap(bitmap);

                // Конвертируем в byte[] для сохранения
                ByteArrayOutputStream stream = new ByteArrayOutputStream();
                bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
                newPhotoData = stream.toByteArray();

            } catch (IOException e) {
                Toast.makeText(this, "Ошибка загрузки фото", Toast.LENGTH_SHORT).show();
            }
        }
    }

    private void saveProfile() {
        String firstName = etFirstName.getText().toString().trim();
        String lastName = etLastName.getText().toString().trim();
        String ageStr = etAge.getText().toString().trim();
        String phone = etPhone.getText().toString().trim();

        if (firstName.isEmpty() || lastName.isEmpty()) {
            Toast.makeText(this, "Заполните имя и фамилию", Toast.LENGTH_SHORT).show();
            return;
        }

        progressBar.setVisibility(View.VISIBLE);
        btnSave.setEnabled(false);

        // Имитация сохранения профиля
        new Handler().postDelayed(() -> {
            // Обновляем данные пользователя
            currentUser.setFirstName(firstName);
            currentUser.setLastName(lastName);
            currentUser.setPhone(phone);

            if (!ageStr.isEmpty()) {
                try {
                    currentUser.setAge(Integer.parseInt(ageStr));
                } catch (NumberFormatException e) {
                    // Игнорируем
                }
            }

            if (newPhotoData != null) {
                currentUser.setPhoto(newPhotoData);
            }

            // Сохраняем в сессию
            sessionManager.saveUser(currentUser);

            progressBar.setVisibility(View.GONE);
            btnSave.setEnabled(true);

            Toast.makeText(ProfileActivity.this, "Профиль обновлен", Toast.LENGTH_SHORT).show();
        }, 1000);
    }

    private void showChangePasswordDialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Изменение пароля");

        View dialogView = getLayoutInflater().inflate(R.layout.dialog_change_password, null);
        builder.setView(dialogView);

        EditText etOldPassword = dialogView.findViewById(R.id.etOldPassword);
        EditText etNewPassword = dialogView.findViewById(R.id.etNewPassword);
        EditText etConfirmPassword = dialogView.findViewById(R.id.etConfirmPassword);

        builder.setPositiveButton("Сохранить", (dialog, which) -> {
            String oldPass = etOldPassword.getText().toString().trim();
            String newPass = etNewPassword.getText().toString().trim();
            String confirmPass = etConfirmPassword.getText().toString().trim();

            if (oldPass.isEmpty() || newPass.isEmpty() || confirmPass.isEmpty()) {
                Toast.makeText(this, "Заполните все поля", Toast.LENGTH_SHORT).show();
                return;
            }

            if (newPass.length() < 6) {
                Toast.makeText(this, "Новый пароль должен быть не менее 6 символов", Toast.LENGTH_SHORT).show();
                return;
            }

            if (!newPass.equals(confirmPass)) {
                Toast.makeText(this, "Пароли не совпадают", Toast.LENGTH_SHORT).show();
                return;
            }

            // Имитация смены пароля
            Toast.makeText(this, "Пароль успешно изменен", Toast.LENGTH_SHORT).show();
        });

        builder.setNegativeButton("Отмена", null);
        builder.show();
    }

    private void logout() {
        new AlertDialog.Builder(this)
                .setTitle("Выход")
                .setMessage("Вы уверены, что хотите выйти?")
                .setPositiveButton("Выйти", (dialog, which) -> {
                    sessionManager.logout();
                    Intent intent = new Intent(ProfileActivity.this, LoginActivity.class);
                    intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                    startActivity(intent);
                    finish();
                })
                .setNegativeButton("Отмена", null)
                .show();
    }
}