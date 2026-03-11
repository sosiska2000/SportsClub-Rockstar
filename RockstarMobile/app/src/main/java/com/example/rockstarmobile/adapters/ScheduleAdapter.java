package com.example.rockstarmobile.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Schedule;

import java.util.List;

public class ScheduleAdapter extends RecyclerView.Adapter<ScheduleAdapter.ScheduleViewHolder> {

    private final Context context;
    private final List<Schedule> schedules;
    private final OnItemClickListener listener;

    public interface OnItemClickListener {
        void onItemClick(Schedule schedule);
    }

    public ScheduleAdapter(Context context, List<Schedule> schedules, OnItemClickListener listener) {
        this.context = context;
        this.schedules = schedules;
        this.listener = listener;
    }

    @NonNull
    @Override
    public ScheduleViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_schedule, parent, false);
        return new ScheduleViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ScheduleViewHolder holder, int position) {
        Schedule schedule = schedules.get(position);

        holder.tvDirection.setText(schedule.getDirectionName());
        holder.tvService.setText(schedule.getServiceName());
        holder.tvTrainer.setText(schedule.getTrainerName());

        // Исправление: не конкатенируем строки напрямую, используем форматирование
        String timeText = schedule.getDateDisplay() + " " + schedule.getTimeRange();
        holder.tvTime.setText(timeText);

        holder.tvPrice.setText(schedule.getPriceDisplay());
        holder.tvParticipants.setText(schedule.getParticipantsDisplay());

        holder.itemView.setOnClickListener(v -> listener.onItemClick(schedule));

        // Если мест нет, показываем другим цветом
        if (!schedule.isAvailable()) {
            holder.tvParticipants.setTextColor(ContextCompat.getColor(context, android.R.color.holo_red_dark));
        } else {
            holder.tvParticipants.setTextColor(ContextCompat.getColor(context, R.color.fitness));
        }
    }

    @Override
    public int getItemCount() {
        return schedules.size();
    }

    public static class ScheduleViewHolder extends RecyclerView.ViewHolder {
        public CardView cardView;
        public TextView tvDirection, tvService, tvTrainer, tvTime, tvPrice, tvParticipants;

        public ScheduleViewHolder(@NonNull View itemView) {
            super(itemView);
            cardView = itemView.findViewById(R.id.cardView);
            tvDirection = itemView.findViewById(R.id.tvDirection);
            tvService = itemView.findViewById(R.id.tvService);
            tvTrainer = itemView.findViewById(R.id.tvTrainer);
            tvTime = itemView.findViewById(R.id.tvTime);
            tvPrice = itemView.findViewById(R.id.tvPrice);
            tvParticipants = itemView.findViewById(R.id.tvParticipants);
        }
    }
}