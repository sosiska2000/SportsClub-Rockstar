package com.example.rockstarmobile.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Service;

import java.util.List;

public class ServicesAdapter extends RecyclerView.Adapter<ServicesAdapter.ServiceViewHolder> {

    private Context context;
    private List<Service> services;

    public ServicesAdapter(Context context, List<Service> services) {
        this.context = context;
        this.services = services;
    }

    @NonNull
    @Override
    public ServiceViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_service, parent, false);
        return new ServiceViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ServiceViewHolder holder, int position) {
        if (services != null && position < services.size()) {
            Service service = services.get(position);

            holder.tvServiceName.setText(service.getName() != null ? service.getName() : "");
            holder.tvServiceDescription.setText(service.getDescription() != null ? service.getDescription() : "");
            holder.tvServicePrice.setText(service.getPriceDisplay());

            if (service.getDurationMinutes() != null) {
                holder.tvServiceDuration.setText(service.getDurationMinutes() + " мин");
                holder.tvServiceDuration.setVisibility(View.VISIBLE);
            } else {
                holder.tvServiceDuration.setVisibility(View.GONE);
            }
        }
    }

    @Override
    public int getItemCount() {
        return services != null ? services.size() : 0;
    }

    static class ServiceViewHolder extends RecyclerView.ViewHolder {
        TextView tvServiceName, tvServiceDescription, tvServiceDuration, tvServicePrice;

        ServiceViewHolder(@NonNull View itemView) {
            super(itemView);
            tvServiceName = itemView.findViewById(R.id.tvServiceName);
            tvServiceDescription = itemView.findViewById(R.id.tvServiceDescription);
            tvServiceDuration = itemView.findViewById(R.id.tvServiceDuration);
            tvServicePrice = itemView.findViewById(R.id.tvServicePrice);
        }
    }
}