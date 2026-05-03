#!/usr/bin/env bash
set -e

BOOTSTRAP="${KAFKA_BOOTSTRAP:-kafka:29092}"

echo ">>> Listing existing topics"
kafka-topics --bootstrap-server "$BOOTSTRAP" --list

echo ">>> Creating topic: events"
kafka-topics --bootstrap-server "$BOOTSTRAP" --create --if-not-exists \
  --topic space-events \
  --partitions 3 \
  --replication-factor 1 \
  --config retention.ms=604800000

echo ">>> Done. Current topics:"
kafka-topics --bootstrap-server "$BOOTSTRAP" --list