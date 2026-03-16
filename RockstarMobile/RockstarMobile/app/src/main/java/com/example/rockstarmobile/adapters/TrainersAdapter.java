package com.example.rockstarmobile.adapters;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Trainer;

import java.util.List;

public class TrainersAdapter extends RecyclerView.Adapter<TrainersAdapter.TrainerViewHolder> {

    private Context context;
    private List<Trainer> trainers;

    public TrainersAdapter(Context context, List<Trainer> trainers) {
        this.context = context;
        this.trainers = trainers;
    }

    @NonNull
    @Override
    public TrainerViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_trainer, parent, false);
        return new TrainerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull TrainerViewHolder holder, int position) {
        Trainer trainer = trainers.get(position);

        holder.tvName.setText(trainer.getFullName());
        holder.tvDirection.setText(trainer.getDirectionName());
        holder.tvExperience.setText(trainer.getExperienceDisplay());
        holder.tvDescription.setText(trainer.getDescription());

        // Заглушка для фото
        holder.ivPhoto.setImageResource(R.drawable.default_avatar);

        // Яркие цвета для карточек в зависимости от направления
        int cardColor;
        int textColor = Color.WHITE;
        int descriptionColor = Color.parseColor("#F0F0F0");

        String direction = trainer.getDirectionName();
        if (direction != null) {
            switch (direction) {
                case "Йога":
                    cardColor = ContextCompat.getColor(context, R.color.yoga);
                    break;
                case "Фитнес":
                    cardColor = ContextCompat.getColor(context, R.color.fitness);
                    break;
                case "Скалолазание":
                    cardColor = ContextCompat.getColor(context, R.color.climbing);
                    break;
                default:
                    cardColor = ContextCompat.getColor(context, R.color.primary);
                    break;
            }
        } else {
            cardColor = ContextCompat.getColor(context, R.color.primary);
        }

        holder.cardView.setCardBackgroundColor(cardColor);

        // Устанавливаем белый цвет для всех текстовых элементов
        holder.tvName.setTextColor(textColor);
        holder.tvDirection.setTextColor(textColor);
        holder.tvExperience.setTextColor(textColor);
        holder.tvDescription.setTextColor(descriptionColor);
    }

    @Override
    public int getItemCount() {
        return trainers.size();
    }

    static class TrainerViewHolder extends RecyclerView.ViewHolder {
        CardView cardView;
        ImageView ivPhoto;
        TextView tvName, tvDirection, tvExperience, tvDescription;

        TrainerViewHolder(@NonNull View itemView) {
            super(itemView);
            cardView = itemView.findViewById(R.id.cardView);
            ivPhoto = itemView.findViewById(R.id.ivPhoto);
            tvName = itemView.findViewById(R.id.tvName);
            tvDirection = itemView.findViewById(R.id.tvDirection);
            tvExperience = itemView.findViewById(R.id.tvExperience);
            tvDescription = itemView.findViewById(R.id.tvDescription);
        }
    }
}