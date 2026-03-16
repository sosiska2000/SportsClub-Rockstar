package com.example.rockstarmobile.models;

public class PaymentMethod {
    private int id;
    private String type; // card, cash, qr
    private String cardNumber;
    private String cardHolder;
    private String expiryDate;
    private boolean isDefault;

    public PaymentMethod() {}

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getType() { return type; }
    public void setType(String type) { this.type = type; }

    public String getCardNumber() { return cardNumber; }
    public void setCardNumber(String cardNumber) { this.cardNumber = cardNumber; }

    public String getCardHolder() { return cardHolder; }
    public void setCardHolder(String cardHolder) { this.cardHolder = cardHolder; }

    public String getExpiryDate() { return expiryDate; }
    public void setExpiryDate(String expiryDate) { this.expiryDate = expiryDate; }

    public boolean isDefault() { return isDefault; }
    public void setDefault(boolean aDefault) { isDefault = aDefault; }

    public String getMaskedCardNumber() {
        if (cardNumber == null || cardNumber.length() < 4) return "";
        return "**** **** **** " + cardNumber.substring(cardNumber.length() - 4);
    }
}