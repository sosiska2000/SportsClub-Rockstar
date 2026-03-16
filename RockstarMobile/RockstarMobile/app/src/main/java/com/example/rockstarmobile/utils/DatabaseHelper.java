package com.example.rockstarmobile.utils;

import android.os.AsyncTask;
import android.util.Log;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.concurrent.CountDownLatch;

public class DatabaseHelper {

    private static final String TAG = "DatabaseHelper";

    // Данные для подключения к БД - ЗАМЕНИТЕ НА СВОИ!
    private static final String DB_HOST = "10.0.2.2"; // localhost для эмулятора
    private static final String DB_PORT = "3306";
    private static final String DB_NAME = "rockstar_club";
    private static final String DB_USER = "root";
    private static final String DB_PASSWORD = ""; // ваш пароль

    private static final String DB_URL = "jdbc:mysql://" + DB_HOST + ":" + DB_PORT + "/" + DB_NAME +
            "?useSSL=false&serverTimezone=UTC&characterEncoding=utf8";

    private Connection connection;

    public interface QueryCallback {
        void onSuccess(ResultSet resultSet);
        void onError(String error);
    }

    public interface UpdateCallback {
        void onSuccess(int rowsAffected);
        void onError(String error);
    }

    /**
     * Подключение к базе данных
     */
    public void connect(ConnectionCallback callback) {
        new ConnectTask(callback).execute();
    }

    /**
     * Выполнение SELECT запроса
     */
    public void executeQuery(String query, QueryCallback callback) {
        new QueryTask(query, callback).execute();
    }

    /**
     * Выполнение INSERT/UPDATE/DELETE запроса с параметрами
     */
    public void executeUpdate(String query, String[] params, UpdateCallback callback) {
        new UpdateTask(query, params, callback).execute();
    }

    /**
     * Закрытие соединения
     */
    public void disconnect() {
        try {
            if (connection != null && !connection.isClosed()) {
                connection.close();
            }
        } catch (SQLException e) {
            Log.e(TAG, "Error closing connection: " + e.getMessage());
        }
    }

    // Callback интерфейс для подключения
    public interface ConnectionCallback {
        void onSuccess();
        void onError(String error);
    }

    // AsyncTask для подключения
    private class ConnectTask extends AsyncTask<Void, Void, Boolean> {
        private ConnectionCallback callback;
        private String errorMessage;

        ConnectTask(ConnectionCallback callback) {
            this.callback = callback;
        }

        @Override
        protected Boolean doInBackground(Void... voids) {
            try {
                Class.forName("com.mysql.jdbc.Driver");
                connection = DriverManager.getConnection(DB_URL, DB_USER, DB_PASSWORD);
                return true;
            } catch (ClassNotFoundException e) {
                errorMessage = "MySQL Driver not found: " + e.getMessage();
                Log.e(TAG, errorMessage);
            } catch (SQLException e) {
                errorMessage = "SQL Error: " + e.getMessage();
                Log.e(TAG, errorMessage);
            }
            return false;
        }

        @Override
        protected void onPostExecute(Boolean success) {
            if (success) {
                callback.onSuccess();
            } else {
                callback.onError(errorMessage);
            }
        }
    }

    // AsyncTask для SELECT запросов
    private class QueryTask extends AsyncTask<Void, Void, ResultSet> {
        private String query;
        private QueryCallback callback;
        private String errorMessage;

        QueryTask(String query, QueryCallback callback) {
            this.query = query;
            this.callback = callback;
        }

        @Override
        protected ResultSet doInBackground(Void... voids) {
            try {
                if (connection == null || connection.isClosed()) {
                    // Пытаемся подключиться, если соединение закрыто
                    CountDownLatch latch = new CountDownLatch(1);
                    final boolean[] connected = {false};
                    final String[] connError = {null};

                    connect(new ConnectionCallback() {
                        @Override
                        public void onSuccess() {
                            connected[0] = true;
                            latch.countDown();
                        }

                        @Override
                        public void onError(String error) {
                            connError[0] = error;
                            latch.countDown();
                        }
                    });

                    latch.await();

                    if (!connected[0]) {
                        errorMessage = connError[0];
                        return null;
                    }
                }

                Statement statement = connection.createStatement();
                return statement.executeQuery(query);

            } catch (Exception e) {
                errorMessage = e.getMessage();
                Log.e(TAG, "Query error: " + errorMessage);
                return null;
            }
        }

        @Override
        protected void onPostExecute(ResultSet resultSet) {
            if (errorMessage == null) {
                callback.onSuccess(resultSet);
            } else {
                callback.onError(errorMessage);
            }
        }
    }

    // AsyncTask для INSERT/UPDATE/DELETE запросов
    private class UpdateTask extends AsyncTask<Void, Void, Integer> {
        private String query;
        private String[] params;
        private UpdateCallback callback;
        private String errorMessage;

        UpdateTask(String query, String[] params, UpdateCallback callback) {
            this.query = query;
            this.params = params;
            this.callback = callback;
        }

        @Override
        protected Integer doInBackground(Void... voids) {
            try {
                if (connection == null || connection.isClosed()) {
                    CountDownLatch latch = new CountDownLatch(1);
                    final boolean[] connected = {false};
                    final String[] connError = {null};

                    connect(new ConnectionCallback() {
                        @Override
                        public void onSuccess() {
                            connected[0] = true;
                            latch.countDown();
                        }

                        @Override
                        public void onError(String error) {
                            connError[0] = error;
                            latch.countDown();
                        }
                    });

                    latch.await();

                    if (!connected[0]) {
                        errorMessage = connError[0];
                        return -1;
                    }
                }

                PreparedStatement statement = connection.prepareStatement(query);

                if (params != null) {
                    for (int i = 0; i < params.length; i++) {
                        statement.setString(i + 1, params[i]);
                    }
                }

                return statement.executeUpdate();

            } catch (Exception e) {
                errorMessage = e.getMessage();
                Log.e(TAG, "Update error: " + errorMessage);
                return -1;
            }
        }

        @Override
        protected void onPostExecute(Integer rowsAffected) {
            if (errorMessage == null) {
                callback.onSuccess(rowsAffected);
            } else {
                callback.onError(errorMessage);
            }
        }
    }
}