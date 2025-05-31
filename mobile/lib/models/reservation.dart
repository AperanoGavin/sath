import 'package:json_annotation/json_annotation.dart';

part 'reservation.g.dart';

@JsonSerializable()
class Reservation {
  final String id;
  final String spotId;
  final String userId;
  final DateTime createdAt;
  final DateTime from;
  final DateTime to;
  final String status;

  Reservation({
    required this.id,
    required this.spotId,
    required this.userId,
    required this.createdAt,
    required this.from,
    required this.to,
    required this.status,
  });

  factory Reservation.fromJson(Map<String, dynamic> json) =>
      _$ReservationFromJson(json);

  Map<String, dynamic> toJson() => _$ReservationToJson(this);
}
