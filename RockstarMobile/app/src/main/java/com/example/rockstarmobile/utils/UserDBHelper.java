package com.example.rockstarmobile.utils;

import android.content.Context;

import com.example.rockstarmobile.models.User;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

public class UserDBHelper {

    private static final String TAG = "UserDBHelper";
    private DatabaseHelper dbHelper;
    private Context context;

    public interface UserCallback {
        void onSuccess(User user);
        void onError(String error);
    }

    public interface UsersCallback {
        void onSuccess(List<User> users);
        void onError(String error);
    }

    public interface BooleanCallback {
        void onSuccess(boolean result);
        void onError(String error);
    }

    public UserDBHelper(Context context) {
        this.context = context;
        this.dbHelper = new DatabaseHelper();
    }

    /**
     * Регистрация нового пользователя
     */
    public void registerUser(User user, BooleanCallback callback) {
        String query = "INSERT INTO users (email, password_hash, first_name, last_name, phone, age, role, is_active, created_at) " +
                "VALUES (?, ?, ?, ?, ?, ?, 'client', 1, NOW())";

        String[] params = {
                user.getEmail(),
                user.getPasswordHash(), // В реальном проекте нужно хешировать пароль!
                user.getFirstName(),
                user.getLastName(),
                user.getPhone(),
                String.valueOf(user.getAge())
        };

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeUpdate(query, params, new DatabaseHelper.UpdateCallback() {
                    @Override
                    public void onSuccess(int rowsAffected) {
                        callback.onSuccess(rowsAffected > 0);
                        dbHelper.disconnect();
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Авторизация пользователя
     */
    public void loginUser(String email, String password, UserCallback callback) {
        String query = "SELECT * FROM users WHERE email = '" + email + "' AND is_active = 1";

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeQuery(query, new DatabaseHelper.QueryCallback() {
                    @Override
                    public void onSuccess(ResultSet resultSet) {
                        try {
                            if (resultSet.next()) {
                                // В реальном проекте нужно проверять хеш пароля!
                                String dbPassword = resultSet.getString("password_hash");

                                if (password.equals(dbPassword)) {
                                    User user = extractUserFromResultSet(resultSet);
                                    callback.onSuccess(user);
                                } else {
                                    callback.onError("Неверный пароль");
                                }
                            } else {
                                callback.onError("Пользователь не найден");
                            }
                            dbHelper.disconnect();
                        } catch (SQLException e) {
                            callback.onError("Ошибка обработки данных: " + e.getMessage());
                            dbHelper.disconnect();
                        }
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Получение пользователя по ID
     */
    public void getUserById(int userId, UserCallback callback) {
        String query = "SELECT * FROM users WHERE id = " + userId;

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeQuery(query, new DatabaseHelper.QueryCallback() {
                    @Override
                    public void onSuccess(ResultSet resultSet) {
                        try {
                            if (resultSet.next()) {
                                User user = extractUserFromResultSet(resultSet);
                                callback.onSuccess(user);
                            } else {
                                callback.onError("Пользователь не найден");
                            }
                            dbHelper.disconnect();
                        } catch (SQLException e) {
                            callback.onError("Ошибка обработки данных: " + e.getMessage());
                            dbHelper.disconnect();
                        }
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Обновление профиля пользователя
     */
    public void updateUser(User user, BooleanCallback callback) {
        String query = "UPDATE users SET first_name = ?, last_name = ?, phone = ?, age = ? WHERE id = ?";

        String[] params = {
                user.getFirstName(),
                user.getLastName(),
                user.getPhone(),
                String.valueOf(user.getAge()),
                String.valueOf(user.getId())
        };

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeUpdate(query, params, new DatabaseHelper.UpdateCallback() {
                    @Override
                    public void onSuccess(int rowsAffected) {
                        callback.onSuccess(rowsAffected > 0);
                        dbHelper.disconnect();
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Изменение пароля
     */
    public void changePassword(int userId, String newPassword, BooleanCallback callback) {
        String query = "UPDATE users SET password_hash = ? WHERE id = ?";

        String[] params = {
                newPassword, // В реальном проекте нужно хешировать!
                String.valueOf(userId)
        };

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeUpdate(query, params, new DatabaseHelper.UpdateCallback() {
                    @Override
                    public void onSuccess(int rowsAffected) {
                        callback.onSuccess(rowsAffected > 0);
                        dbHelper.disconnect();
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Проверка существования email
     */
    public void checkEmailExists(String email, BooleanCallback callback) {
        String query = "SELECT id FROM users WHERE email = '" + email + "'";

        dbHelper.connect(new DatabaseHelper.ConnectionCallback() {
            @Override
            public void onSuccess() {
                dbHelper.executeQuery(query, new DatabaseHelper.QueryCallback() {
                    @Override
                    public void onSuccess(ResultSet resultSet) {
                        try {
                            callback.onSuccess(resultSet.next());
                            dbHelper.disconnect();
                        } catch (SQLException e) {
                            callback.onError("Ошибка проверки email");
                            dbHelper.disconnect();
                        }
                    }

                    @Override
                    public void onError(String error) {
                        callback.onError(error);
                        dbHelper.disconnect();
                    }
                });
            }

            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }

    /**
     * Извлечение пользователя из ResultSet
     */
    private User extractUserFromResultSet(ResultSet rs) throws SQLException {
        User user = new User();
        user.setId(rs.getInt("id"));
        user.setEmail(rs.getString("email"));
        user.setPasswordHash(rs.getString("password_hash"));
        user.setFirstName(rs.getString("first_name"));
        user.setLastName(rs.getString("last_name"));
        user.setPhone(rs.getString("phone"));
        user.setAge(rs.getInt("age"));
        user.setActive(rs.getBoolean("is_active"));
        user.setCreatedAt(rs.getString("created_at"));

        // Фото пока не загружаем (BLOB)

        return user;
    }
}