// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reservation.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Reservation _$ReservationFromJson(Map<String, dynamic> json) => Reservation(
  id: json['id'] as String,
  spotId: json['spotId'] as String,
  userId: json['userId'] as String,
  createdAt: DateTime.parse(json['createdAt'] as String),
  from: DateTime.parse(json['from'] as String),
  to: DateTime.parse(json['to'] as String),
  status: json['status'] as String,
);

Map<String, dynamic> _$ReservationToJson(Reservation instance) =>
    <String, dynamic>{
      'id': instance.id,
      'spotId': instance.spotId,
      'userId': instance.userId,
      'createdAt': instance.createdAt.toIso8601String(),
      'from': instance.from.toIso8601String(),
      'to': instance.to.toIso8601String(),
      'status': instance.status,
    };
