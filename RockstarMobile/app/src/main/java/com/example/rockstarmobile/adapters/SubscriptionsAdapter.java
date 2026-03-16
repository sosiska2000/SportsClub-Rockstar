package com.example.rockstarmobile.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.rockstarmobile.R;
import com.example.rockstarmobile.models.Subscription;

import java.util.List;

public class SubscriptionsAdapter extends RecyclerView.Adapter<SubscriptionsAdapter.SubscriptionViewHolder> {

    private Context context;
    private List<Subscription> subscriptions;

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
        if (subscriptions != null && position < subscriptions.size()) {
            Subscription subscription = subscriptions.get(position);

            holder.tvSubscriptionName.setText(subscription.getName() != null ? subscription.getName() : "");
            holder.tvSubscriptionDescription.setText(subscription.getDescription() != null ? subscription.getDescription() : "");
            holder.tvSubscriptionPrice.setText(subscription.getPriceDisplay());
            holder.tvSubscriptionSessions.setText(subscription.getSessionsDisplay());
        }
    }

    @Override
    public int getItemCount() {
        return subscriptions != null ? subscriptions.size() : 0;
    }

    static class SubscriptionViewHolder extends RecyclerView.ViewHolder {
        TextView tvSubscriptionName, tvSubscriptionDescription, tvSubscriptionSessions, tvSubscriptionPrice;

        SubscriptionViewHolder(@NonNull View itemView) {
            super(itemView);
            tvSubscriptionName = itemView.findViewById(R.id.tvSubscriptionName);
            tvSubscriptionDescription = itemView.findViewById(R.id.tvSubscriptionDescription);
            tvSubscriptionSessions = itemView.findViewById(R.id.tvSubscriptionSessions);
            tvSubscriptionPrice = itemView.findViewById(R.id.tvSubscriptionPrice);
        }
    }
}