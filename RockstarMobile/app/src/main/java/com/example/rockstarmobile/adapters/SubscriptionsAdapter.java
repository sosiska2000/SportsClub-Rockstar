package com.example.rockstarmobile.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Subscription;

import java.util.List;

public class SubscriptionsAdapter extends RecyclerView.Adapter<SubscriptionsAdapter.SubscriptionViewHolder> {

    private final Context context;
    private final List<Subscription> subscriptions;

    public SubscriptionsAdapter(Context context, List<Subscription> subscriptions) {
        this.context = context;
        this.subscriptions = subscriptions;
    }

    @NonNull
    @Override
    public SubscriptionViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_subscription, parent, false);
        return new SubscriptionViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull SubscriptionViewHolder holder, int position) {
        Subscription subscription = subscriptions.get(position);

        holder.tvName.setText(subscription.getName());
        holder.tvPrice.setText(subscription.getPriceDisplay());
        holder.tvSessions.setText(subscription.getSessionsDisplay());

        if (subscription.getDescription() != null && !subscription.getDescription().isEmpty()) {
            holder.tvDescription.setText(subscription.getDescription());
            holder.tvDescription.setVisibility(View.VISIBLE);
        } else {
            holder.tvDescription.setVisibility(View.GONE);
        }
    }

    @Override
    public int getItemCount() {
        return subscriptions.size();
    }

    public static class SubscriptionViewHolder extends RecyclerView.ViewHolder {
        public CardView cardView;
        public TextView tvName, tvPrice, tvSessions, tvDescription;

        public SubscriptionViewHolder(@NonNull View itemView) {
            super(itemView);
            cardView = itemView.findViewById(R.id.cardView);
            tvName = itemView.findViewById(R.id.tvName);
            tvPrice = itemView.findViewById(R.id.tvPrice);
            tvSessions = itemView.findViewById(R.id.tvSessions);
            tvDescription = itemView.findViewById(R.id.tvDescription);
        }
    }
}